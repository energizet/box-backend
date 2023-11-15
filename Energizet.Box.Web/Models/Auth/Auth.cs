using System.Text.Json.Serialization;

namespace Energizet.Box.Web.Models.Auth;

public class AuthRequest
{
	public static readonly AuthRequest Empty = new();

	public int Uid { get; set; }
	[JsonPropertyName("first_name")]
	public string FirstName { get; set; } = string.Empty;
	[JsonPropertyName("last_name")]
	public string LastName { get; set; } = string.Empty;
	public string Photo { get; set; } = string.Empty;
	[JsonPropertyName("photo_rec")]
	public string PhotoRec { get; set; } = string.Empty;
	public string Hash { get; set; } = string.Empty;
}

public class AuthResponse
{
	public string Token { get; set; } = string.Empty;
}