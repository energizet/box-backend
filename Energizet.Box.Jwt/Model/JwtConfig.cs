namespace Energizet.Box.Jwt.Model;

public class JwtConfig
{
	public string Issuer { get; set; } = string.Empty;
	public string Audience { get; set; } = string.Empty;
	public double ExpiresInMinutes { get; set; }
	public string SecurityKey => "123456789123456789";
}