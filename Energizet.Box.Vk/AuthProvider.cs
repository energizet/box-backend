using System.Security;
using Energizet.Box.Vk.Model;

namespace Energizet.Box.Vk;

public class AuthProvider : IDisposable
{
	private readonly VkConfig _vkConfig;
	private readonly HashProvider _hashProvider;

	public AuthProvider(VkConfig vkConfig, HashProvider hashProvider)
	{
		_vkConfig = vkConfig;
		_hashProvider = hashProvider;
	}

	public async Task<object> AuthAsync(int vkUserId, string hash, CancellationToken token)
	{
		var secretUserStr = _vkConfig.AppId + vkUserId + _vkConfig.SecretKey;

		if (_hashProvider.VerifyHash(secretUserStr, hash) == false)
		{
			throw new VerificationException("Incorrect hash");
		}

		return new
		{
			Ok = true,
		};
	}

	public void Dispose()
	{
		_hashProvider.Dispose();
	}
}