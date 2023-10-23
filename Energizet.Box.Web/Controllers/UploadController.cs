using System.Net.Http.Headers;
using System.Text.Json;
using Energizet.Box.Entities;
using Energizet.Box.Web.Models;
using Energizet.Box.Web.Models.Upload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Energizet.Box.Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UploadController : ControllerBase
	{
		private readonly VkConfig _vkConfig;

		public UploadController(IOptions<VkConfig> vkConfig)
		{
			_vkConfig = vkConfig.Value;
		}

		[HttpPost]
		public async Task<ActionResult<UploadResponse>> Post(IFormFile file,
			CancellationToken token)
		{
			var id = Guid.NewGuid();

			if (Directory.Exists("./tmp") == false)
			{
				Directory.CreateDirectory("./tmp");
			}

			await using var tmpFile = System.IO.File.Create($"./tmp/{id}");
			await file.CopyToAsync(tmpFile, token);

			return new UploadResponse
			{
				Id = id,
			};
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> Save(SaveRequest saveRequest,
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

			var vkId = saveRequest.VkLink.Split("/").Last();

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", _vkConfig.ServiceKey);
			using var formData = new MultipartFormDataContent();
			formData.Add(new StringContent(vkId), "user_ids");
			formData.Add(new StringContent("photo"), "fields");
			formData.Add(new StringContent("5.154"), "v");

			var res = await client.PostAsync("https://api.vk.com/method/users.get", formData,
				token);
			var vkUserJson = await res.Content.ReadAsStringAsync(token);
			var vkUser = JsonSerializer.Deserialize<VkResponse<VkUser>>(vkUserJson)?.Response
				.FirstOrDefault();

			if (vkUser == null)
			{
				return BadRequest();
			}

			await System.IO.File.AppendAllLinesAsync(
				"./db/files.txt",
				new[] { $"{saveRequest.Id}|{saveRequest.Title}|{vkUser.Id}" },
				token
			);

			return Ok();
		}
	}
}