using Energizet.Box.Db;
using Energizet.Box.Db.Model;
using Energizet.Box.FileStore;
using Energizet.Box.Vk;
using Energizet.Box.Web.Models.File;
using Microsoft.AspNetCore.Mvc;

namespace Energizet.Box.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase, IDisposable
{
	private readonly FileProvider _fileProvider;
	private readonly VkProvider _vkProvider;
	private readonly DbProvider _dbProvider;

	public FileController(FileProvider fileProvider, VkProvider vkProvider, DbProvider dbProvider)
	{
		_fileProvider = fileProvider;
		_vkProvider = vkProvider;
		_dbProvider = dbProvider;
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<UploadResponse>> Upload(
		IFormFile file, CancellationToken token
	)
	{
		var id = Guid.NewGuid();

		await using var tmpFile = await _fileProvider.NewAsync(id);
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

			if (vkUser == null)
			{
				return BadRequest();
			}

			await _fileProvider.SaveAsync(saveRequest.Id);
			await _dbProvider.SaveAsync(saveRequest.Id, saveRequest.Title, vkUser.Id, token);

			return new
			{
				Ok = true,
			};
		}
		catch (ArgumentException ex)
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
		catch (ArgumentException ex)
		{
			return NotFound(ex);
		}
	}

	public void Dispose()
	{
		_vkProvider.Dispose();
	}
}