using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using TemplateIHD.Domain.Exceptions;

namespace TemplateIHD.WebAPI.Extensions
{
    [ExcludeFromCodeCoverage]
    public class ExceptionMiddlewareExtensions : ExceptionFilterAttribute
    {
        public IWebHostEnvironment HostingEnvironment { get; }
        public TelemetryClient TelemetryClient { get; }

        public ExceptionMiddlewareExtensions(IWebHostEnvironment hostingEnvironment, TelemetryClient telemetryClient)
        {
            HostingEnvironment = hostingEnvironment;
            TelemetryClient = telemetryClient;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context != null)
            {
                if (context.Exception is HttpBadRequestException)
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Result = new JsonResult(context.Exception.Message);
                }
                else
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Result = new JsonResult(context.Exception.Message);
                }

                if (!HostingEnvironment.IsDevelopment())
                {
                    // Report exception to insights
                    TelemetryClient.TrackException(context.Exception);
                    TelemetryClient.Flush();
                }
            }
            base.OnException(context);
        }
    }
}
