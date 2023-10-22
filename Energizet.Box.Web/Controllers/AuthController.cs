using Energizet.Box.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Energizet.Box.Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		[HttpPost]
		public User Post(User user)
		{
			return user;
		}
	}
}