using System.Security.Cryptography;
using System.Text;
using Energizet.Box.Core;
using Energizet.Box.Db;
using Energizet.Box.Db.Abstractions;
using Energizet.Box.FileStore;
using Energizet.Box.Jwt;
using Energizet.Box.Jwt.Abstractions;
using Energizet.Box.Jwt.Model;
using Energizet.Box.Store.Abstraction;
using Energizet.Box.Vk;
using Energizet.Box.Vk.Abstractions;
using Energizet.Box.Vk.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Energizet.Box.Di;

public static class DiExtensions
{
	public static IServiceCollection AddConfiguration(this IServiceCollection collection,
		IConfiguration configuration)
	{
		collection.AddSingleton<VkConfig>(
			_ => configuration.GetSection(nameof(VkConfig)).Get<VkConfig>()
				?? throw new NullReferenceException($"Section {nameof(VkConfig)} not found")
		);
		collection.AddSingleton<JwtConfig>(
			_ => configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>()
				?? throw new NullReferenceException($"Section {nameof(JwtConfig)} not found")
		);

		return collection;
	}

	public static IServiceCollection AddDi(this IServiceCollection collection)
	{
		collection.AddScoped<HashProvider>(_ => new HashProvider(MD5.Create()));
		collection.AddScoped<IAuthProvider, AuthProvider>();
		collection.AddScoped<IStoreProvider, FileStoreProvider>(
			_ => new FileStoreProvider("./tmp", "./store")
		);
		collection.AddScoped<IVkProvider, VkProvider>();
		collection.AddScoped<IDbProvider, DbProvider>();
		collection.AddScoped<IJwtProvider, JwtProvider>();

		collection.AddScoped<AuthCases>();
		collection.AddScoped<FileCases>();

		collection.AddTransient<HttpClient>();

		return collection;
	}

	public static IServiceCollection AddAuth(this IServiceCollection collection,
		IConfiguration configuration)
	{
		var jwtConfig = configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>()!;
		collection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(jwtOptions =>
			{
				jwtOptions.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuers = new[] { jwtConfig.Issuer },
					ValidateAudience = true,
					ValidAudiences = new[] { jwtConfig.Audience },
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey =
						new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecurityKey)),
				};
				jwtOptions.RequireHttpsMetadata = false;
			});
		collection.AddAuthorization(options =>
		{
			options.DefaultPolicy =
				new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
					.RequireAuthenticatedUser()
					.Build();
		});

		return collection;
	}
}