using System.Security.Claims;
using Energizet.Box.Core;
using Energizet.Box.Exceptions;
using Energizet.Box.Web.Models.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Energizet.Box.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class FileController : ControllerBase
{
	private readonly FileCases _fileCases;

	public FileController(FileCases fileCases)
	{
		_fileCases = fileCases;
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<UploadResponse>> Upload(
		IFormFile file, CancellationToken token
	)
	{
		await using var streamFile = new MemoryStream();
		await file.CopyToAsync(streamFile, token);
		streamFile.Position = 0;

		var id = await _fileCases.CreateAsync(streamFile, file.ContentType, token);

		return new UploadResponse
		{
			Id = id,
		};
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<SaveResponse>> Save(
		SaveRequest request, CancellationToken token
	)
	{
		try
		{
			await _fileCases.SaveAsync(request.Id, request.Title, request.VkLink, token);

			return new SaveResponse
			{
				Ok = true,
			};
		}
		catch (NotFoundException ex)
		{
			return NotFound(ex.ToString());
		}
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<InfoResponse>> Info(Guid id, CancellationToken token)
	{
		try
		{
			var info = await _fileCases.InfoAsync(id, token);

			return new InfoResponse
			{
				Id = info.Id,
				Title = info.Title,
				VkUser = info.VkUser,
			};
		}
		catch (NotFoundException ex)
		{
			return NotFound(ex.ToString());
		}
	}

	[HttpGet("{id:guid}/[action]")]
	[Authorize(Roles = "user")]
	public async Task<IActionResult> Download(Guid id, CancellationToken token)
	{
		try
		{
			var vkUserId = HttpContext.User.FindFirst(ClaimTypes.Sid)!.Value;
			var file = await _fileCases.GetAsync(id, vkUserId, token);
			var stream = file.Stream;
			return File(stream, file.ContentType, file.FileName);
		}
		catch (NotFoundException ex)
		{
			return NotFound(ex.ToString());
		}
		catch (ForbiddenException)
		{
			return Forbid();
		}
	}
}