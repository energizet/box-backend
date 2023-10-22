using System.Security.Cryptography;
using System.Text;
using Energizet.Box.Web.Models;
using Energizet.Box.Web.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Energizet.Box.Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly VkConfig _vkConfig;

		public AuthController(IOptions<VkConfig> vkConfig)
		{
			_vkConfig = vkConfig.Value;
		}

		[HttpPost]
		public ActionResult<User> Post(User user)
		{
			var secretUserStr = _vkConfig.AppId + user.Uid + _vkConfig.SecretKey;

			using var md5 = MD5.Create();
			if (VerifyHash(md5, secretUserStr, user.Hash) == false)
			{
				return BadRequest();
			}

			return user;
		}

		private static string GetHash(HashAlgorithm hashAlgorithm, string input)
		{
			var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
			return string.Join("", data.Select(x => x.ToString("x2")));
		}

		private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
		{
			var hashOfInput = GetHash(hashAlgorithm, input);
			return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hash) == 0;
		}
	}
}