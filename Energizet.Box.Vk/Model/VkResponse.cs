using System.Text.Json.Serialization;

namespace Energizet.Box.Vk.Model;

public class VkResponse<T>
{
	[JsonPropertyName("response")]
	public T[] Response { get; set; } = Array.Empty<T>();
}

public class VkUser
{
	[JsonPropertyName("id")]
	public int Id { get; set; }

	[JsonPropertyName("first_name")]
	public string FirstName { get; set; } = string.Empty;

	[JsonPropertyName("last_name")]
	public string LastName { get; set; } = string.Empty;

	[JsonPropertyName("photo")]
	public string Photo { get; set; } = string.Empty;
}