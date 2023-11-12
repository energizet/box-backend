using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Energizet.Box.Jwt.Abstractions;
using Energizet.Box.Jwt.Model;
using Microsoft.IdentityModel.Tokens;

namespace Energizet.Box.Jwt;

public sealed class JwtProvider : IJwtProvider
{
	private readonly JwtSecurityTokenHandler _handler = new();
	private readonly JwtConfig _jwtConfig;

	public JwtProvider(JwtConfig jwtConfig)
	{
		_jwtConfig = jwtConfig;
	}

	public string CreateToken(int vkUserId)
	{
		var jwt = new JwtSecurityToken(
			_jwtConfig.Issuer,
			_jwtConfig.Audience,
			new[]
			{
				new Claim(ClaimTypes.Sid, vkUserId.ToString(), ClaimValueTypes.Integer),
				new Claim(ClaimTypes.Name, vkUserId.ToString(), ClaimValueTypes.Integer),
				new Claim(ClaimTypes.Role, "user"),
			},
			expires: DateTime.Now.Add(TimeSpan.FromMinutes(_jwtConfig.ExpiresInMinutes)),
			signingCredentials: new SigningCredentials(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecurityKey)),
				SecurityAlgorithms.HmacSha512
			)
		);

		return _handler.WriteToken(jwt);
	}
}