using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TemplateIHD.CrossCutting.Azure;
using TemplateIHD.Data.Contexts;
using TemplateIHD.WebAPI.Extensions;

namespace TemplateIHD
{
    public class Startup
    {
        private readonly string AllowedHosts = "_IHDTemplateHosts";
        private readonly AzureKeyVaultService KeyVaultService;
        private readonly IWebHostEnvironment _environment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            KeyVaultService = new AzureKeyVaultService(configuration);
            _environment = env;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (!_environment.IsEnvironment("Local"))
            {
                connectionString = KeyVaultService.GetSecret("AzureDbConnection");
            }

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Connection String is null or empty");

            services.AddDbContext<SqlServerContext>(options => options.UseSqlServer(connectionString));

            services.AddAutoMapper(typeof(Startup));

            var swaggerScope = KeyVaultService.GetSecret("SwaggerScope");
            var swaggerAuthorizationUrl = KeyVaultService.GetSecret("SwaggerAuthorizationUrl");
            var swaggerTokenUrl = KeyVaultService.GetSecret("SwaggerTokenUrl");

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "IHD Template",
                    Description = "IHD Template API"
                });

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                c.AddSecurityDefinition("aad-jwt", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(swaggerAuthorizationUrl),
                            Scopes = new Dictionary<string, string>() { { swaggerScope, "Basic Swagger scope" } },
                            TokenUrl = new Uri(swaggerTokenUrl)
                        }
                    },
                });

                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            services.AddCors(options =>
            {
                options.AddPolicy(AllowedHosts,
                    builder =>
                    {
                        builder.WithHeaders(new[] { "Origin", "X-Requested-With", "Content-Type", "Accept", "Authorization" });
                        builder.WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete);
                        builder.AllowAnyOrigin();
                    }
                );
            });

            services.AddControllers(options =>
            {
                options.Filters.Add<ExceptionMiddlewareExtensions>();
            });

            //var MY_API_URL = KeyVaultService.GetSecret("MY_API_URL");

            //services.AddHttpClient("MY_API_URL", c =>
            //{
            //    c.BaseAddress = new Uri(MY_API_URL);
            //    c.DefaultRequestHeaders.Add("Accept", "application/json");
            //    c.DefaultRequestHeaders.Add("User-Agent", "HttpClientEBXApi");
            //});

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                Configuration.Bind("AzureAd", options); // Replace Bind with its KeyVault equivalent
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            Setup(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }

        private void Setup(IServiceCollection services)
        {
            //services.AddServices();
            //services.AddRepositories();
            //services.AddExternalRepositories();
            //services.AddCrossCuttingServices();
        }

    }
}
