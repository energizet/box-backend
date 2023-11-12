using System.Security.Claims;
using Energizet.Box.Core;
using Energizet.Box.Entities;
using Energizet.Box.Exceptions;
using Energizet.Box.Vk.Abstractions;
using Energizet.Box.Web.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Energizet.Box.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController : ControllerBase
{
	private readonly AuthCases _authCases;
	private readonly IVkProvider _vkProvider;

	public AuthController(AuthCases authCases, IVkProvider vkProvider)
	{
		_authCases = authCases;
		_vkProvider = vkProvider;
	}

	[HttpGet("[action]")]
	[Authorize(Roles = "user")]
	public async Task<ActionResult<VkUser>> Info(CancellationToken token)
	{
		try
		{
			var vkUserId = HttpContext.User.FindFirst(ClaimTypes.Sid)!.Value;
			var vkUser = await _vkProvider.GetVkUser(vkUserId, token);
			return vkUser;
		}
		catch (NotFoundException ex)
		{
			return NotFound(ex.ToString());
		}
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