using Energizet.Box.Vk.Abstractions;
using Energizet.Box.Vk.Model;

namespace Energizet.Box.Vk;

public sealed class AuthProvider : IAuthProvider
{
	private readonly VkConfig _vkConfig;
	private readonly HashProvider _hashProvider;

	public AuthProvider(VkConfig vkConfig, HashProvider hashProvider)
	{
		_vkConfig = vkConfig;
		_hashProvider = hashProvider;
	}

	public Task<bool> VerifyHashAsync(int vkUserId, string hash, CancellationToken token)
	{
		var secretUserStr = _vkConfig.AppId + vkUserId + _vkConfig.SecretKey;

		return Task.FromResult(_hashProvider.VerifyHash(secretUserStr, hash));
	}
}