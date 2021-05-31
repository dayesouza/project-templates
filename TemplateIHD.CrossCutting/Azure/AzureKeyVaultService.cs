using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;
using TemplateIHD.CrossCutting.Interfaces;

namespace TemplateIHD.CrossCutting.Azure
{
    [ExcludeFromCodeCoverage]
    internal class AzureKeyVaultService : IAzureKeyVaultService
    {
        protected readonly SecretClient _client;

        public AzureKeyVaultService(IConfiguration configuration)
        {
            try
            {
                var options = new SecretClientOptions()
                {
                    Retry =
                    {
                        Delay = TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential,
                    }
                };

                _client = new SecretClient(new Uri(configuration["AzureVaultUrl"]), new DefaultAzureCredential(), options);
            }
            catch (Exception e)
            {
                throw new Exception("Failed during Azure Key Vault connection.", e);
            }
        }

        public string GetSecret(string key)
        {
            KeyVaultSecret ebxMulesoftClientId = _client.GetSecret(key);
            return ebxMulesoftClientId.Value;
        }
    }
}
