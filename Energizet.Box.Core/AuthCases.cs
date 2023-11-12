using Energizet.Box.Exceptions;
using Energizet.Box.Jwt.Abstractions;
using Energizet.Box.Vk.Abstractions;

namespace Energizet.Box.Core;

public sealed class AuthCases
{
	private readonly IAuthProvider _authProvider;
	private readonly IJwtProvider _jwtProvider;

	public AuthCases(IAuthProvider authProvider, IJwtProvider jwtProvider)
	{
		_authProvider = authProvider;
		_jwtProvider = jwtProvider;
	}

	public async Task<string> AuthAsync(int vkUserId, string hash, CancellationToken token)
	{
		var verifyHash = await _authProvider.VerifyHashAsync(vkUserId, hash, token);

		if (verifyHash == false)
		{
			throw new HashIncorrectExceptions("Incorrect hash");
		}

		var jwtToken = _jwtProvider.CreateToken(vkUserId);

		return jwtToken;
	}
}