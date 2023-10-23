using System.Security;
using Energizet.Box.Vk;
using Energizet.Box.Web.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Energizet.Box.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController : ControllerBase, IDisposable
{
	private readonly AuthProvider _authProvider;

	public AuthController(AuthProvider authProvider)
	{
		_authProvider = authProvider;
	}

	[HttpPost]
	public async Task<ActionResult<object>> Post(User user, CancellationToken token)
	{
		try
		{
			return await _authProvider.AuthAsync(user.Uid, user.Hash, token);
		}
		catch (VerificationException ex)
		{
			return BadRequest(ex);
		}
	}

	public void Dispose()
	{
		_authProvider.Dispose();
	}
}