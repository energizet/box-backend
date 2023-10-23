using System.Security;
using Energizet.Box.Vk.Abstractions;

namespace Energizet.Box.Core;

public sealed class AuthCases
{
	private readonly IAuthProvider _authProvider;

	public AuthCases(IAuthProvider authProvider)
	{
		_authProvider = authProvider;
	}

	public async Task<object> AuthAsync(int vkUserId, string hash, CancellationToken token)
	{
		var verifyHash = await _authProvider.VerifyHashAsync(vkUserId, hash, token);

		if (verifyHash == false)
		{
			throw new VerificationException("Incorrect hash");
		}

		return new
		{
			Ok = true,
		};
	}
}