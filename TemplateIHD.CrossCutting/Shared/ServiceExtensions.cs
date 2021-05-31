using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using TemplateIHD.CrossCutting.Azure;
using TemplateIHD.CrossCutting.Interfaces;

namespace TemplateIHD.CrossCutting
{
	[ExcludeFromCodeCoverage]
	public static class ServiceExtensions
	{
		public static void AddCrossCuttingServices(this IServiceCollection services)
		{
			services.AddTransient<IAzureKeyVaultService, AzureKeyVaultService>();
		}
	}
}
