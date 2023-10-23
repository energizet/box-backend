using System.Security.Cryptography;
using Energizet.Box.Core;
using Energizet.Box.Db;
using Energizet.Box.Db.Abstractions;
using Energizet.Box.FileStore;
using Energizet.Box.Store.Abstraction;
using Energizet.Box.Vk;
using Energizet.Box.Vk.Abstractions;
using Energizet.Box.Vk.Model;
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
		collection.AddScoped<IAuthProvider, AuthProvider>();
		collection.AddScoped<IStoreProvider, FileStoreProvider>(
			_ => new FileStoreProvider("./tmp", "./store")
		);
		collection.AddScoped<IVkProvider, VkProvider>();
		collection.AddScoped<IDbProvider, DbProvider>();

		collection.AddScoped<AuthCases>();
		collection.AddScoped<FileCases>();

		collection.AddTransient<HttpClient>();

		return collection;
	}
}