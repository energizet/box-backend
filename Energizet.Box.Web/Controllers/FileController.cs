using Energizet.Box.Db.Abstractions;
using Energizet.Box.Exceptions;
using Energizet.Box.Store.Abstraction;
using Energizet.Box.Vk.Abstractions;
using Energizet.Box.Web.Models.File;
using Microsoft.AspNetCore.Mvc;

namespace Energizet.Box.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class FileController : ControllerBase
{
	private readonly IStoreProvider _storeProvider;
	private readonly IVkProvider _vkProvider;
	private readonly IDbProvider _dbProvider;

	public FileController(
		IStoreProvider storeProvider, IVkProvider vkProvider, IDbProvider dbProvider
	)
	{
		_storeProvider = storeProvider;
		_vkProvider = vkProvider;
		_dbProvider = dbProvider;
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<UploadResponse>> Upload(
		IFormFile file, CancellationToken token
	)
	{
		var id = Guid.NewGuid();

		await using var tmpFile = await _storeProvider.NewAsync(id);
		await file.CopyToAsync(tmpFile, token);

		return new UploadResponse
		{
			Id = id,
		};
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<object>> Save(SaveRequest saveRequest, CancellationToken token)
	{
		try
		{
			var vkId = saveRequest.VkLink.Split("/").Last();
			var vkUser = await _vkProvider.GetVkUser(vkId, token);

			await _storeProvider.SaveAsync(saveRequest.Id);
			await _dbProvider.SaveAsync(saveRequest.Id, saveRequest.Title, vkUser.Id, token);

			return new
			{
				Ok = true,
			};
		}
		catch (NotFoundException ex)
		{
			return NotFound(ex);
		}
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<InfoResponse>> Info(Guid id, CancellationToken token)
	{
		try
		{
			var fileDb = await _dbProvider.Find(id, token);
			var vkUser = await _vkProvider.GetVkUser(fileDb.VkUserId, token);

			return new InfoResponse
			{
				Id = fileDb.Id,
				Title = fileDb.Title,
				VkUser = vkUser,
			};
		}
		catch (NotFoundException ex)
		{
			return NotFound(ex);
		}
	}
}