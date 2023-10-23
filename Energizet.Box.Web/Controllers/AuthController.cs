using System.Security;
using Energizet.Box.Vk;
using Energizet.Box.Web.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Energizet.Box.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase, IDisposable
{
	private readonly AuthProvider _authProvider;

	public AuthController(AuthProvider authProvider)
	{
		_authProvider = authProvider;
	}

	[HttpPost]
	public ActionResult<object> Post(User user)
	{
		try
		{
			return _authProvider.Auth(user.Uid, user.Hash);
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