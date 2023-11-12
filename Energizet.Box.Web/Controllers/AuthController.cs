using Energizet.Box.Core;
using Energizet.Box.Exceptions;
using Energizet.Box.Web.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Energizet.Box.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController : ControllerBase
{
	private readonly AuthCases _authCases;

	public AuthController(AuthCases authCases)
	{
		_authCases = authCases;
	}

	[HttpPost]
	public async Task<ActionResult<AuthResponse>> Auth(AuthRequest authRequest,
		CancellationToken token)
	{
		try
		{
			var jwt = await _authCases.AuthAsync(authRequest.Uid, authRequest.Hash, token);
			return new AuthResponse
			{
				Token = jwt,
			};
		}
		catch (HashIncorrectExceptions ex)
		{
			return BadRequest(ex.ToString());
		}
	}
}