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
	public async Task<ActionResult<object>> Post(User user, CancellationToken token)
	{
		try
		{
			return await _authCases.AuthAsync(user.Uid, user.Hash, token);
		}
		catch (HashIncorrectExceptions ex)
		{
			return BadRequest(ex);
		}
	}
}