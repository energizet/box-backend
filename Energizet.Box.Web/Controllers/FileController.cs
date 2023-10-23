using Energizet.Box.FileStore;
using Energizet.Box.Vk;
using Energizet.Box.Web.Models.File;
using Energizet.Box.Web.Models.Upload;
using Microsoft.AspNetCore.Mvc;

namespace Energizet.Box.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase, IDisposable
{
	private readonly FileProvider _fileProvider;
	private readonly VkProvider _vkProvider;

	public FileController(FileProvider fileProvider, VkProvider vkProvider)
	{
		_fileProvider = fileProvider;
		_vkProvider = vkProvider;
	}

	[HttpPost("[action]")]
	public async Task<ActionResult<UploadResponse>> Upload(IFormFile file,
		CancellationToken token)
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
	public async Task<IActionResult> Save(SaveRequest saveRequest, CancellationToken token)
	{
		var vkId = saveRequest.VkLink.Split("/").Last();

		var vkUser = await _vkProvider.GetVkUser(vkId, token);

		if (vkUser == null)
		{
			return BadRequest();
		}

		await _fileProvider.SaveAsync(saveRequest.Id);

		if (Directory.Exists("./db") == false)
		{
			Directory.CreateDirectory("./db");
		}

		if (System.IO.File.Exists("./db/files.txt") == false)
		{
			await using var tmpFile = System.IO.File.Create("./db/files.txt");
		}

		await System.IO.File.AppendAllLinesAsync(
			"./db/files.txt",
			new[] { $"{saveRequest.Id}|{saveRequest.Title}|{vkUser.Id}" },
			token
		);

		return Ok();
	}

	[HttpGet("{id:guid}")]
	public async Task<ActionResult<InfoResponse>> Info(Guid id,
		CancellationToken token)
	{
		if (Directory.Exists("./db") == false)
		{
			Directory.CreateDirectory("./db");
		}

		if (System.IO.File.Exists("./db/files.txt") == false)
		{
			await using var tmpFile = System.IO.File.Create("./db/files.txt");
		}

		var files = (await System.IO.File.ReadAllLinesAsync("./db/files.txt", token))
			.Select(line => line.Split("|"))
			.ToList();

		var file = files.FirstOrDefault(line => line[0] == id.ToString());

		if (file == null)
		{
			return NotFound();
		}

		var fileDb = new FileDb
		{
			Id = Guid.Parse(file[0]),
			Title = file[1],
			VkUserId = file[2],
		};

		var vkUser = await _vkProvider.GetVkUser(fileDb.VkUserId, token);

		return new InfoResponse
		{
			Id = fileDb.Id,
			Title = fileDb.Title,
			VkUser = vkUser,
		};
	}

	public void Dispose()
	{
		_vkProvider.Dispose();
	}
}