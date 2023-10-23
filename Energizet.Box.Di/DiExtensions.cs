using System.Security.Cryptography;
using Energizet.Box.Auth;
using Energizet.Box.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Energizet.Box.Di;

public static class DiExtensions
{
	public static IServiceCollection AddConfiguration(this IServiceCollection collection,
		IConfiguration configuration)
	{
		collection.Configure<VkConfig>(configuration.GetSection(nameof(VkConfig)));

		return collection;
	}

	public static IServiceCollection AddDi(this IServiceCollection collection)
	{
		collection.AddSingleton<VkConfig>(
			provider => provider.GetRequiredService<IOptions<VkConfig>>().Value
		);
		collection.AddScoped<HashProvider>(_ => new HashProvider(MD5.Create()));
		collection.AddScoped<AuthProvider>();

		return collection;
	}
}