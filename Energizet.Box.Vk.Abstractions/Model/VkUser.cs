namespace Energizet.Box.Vk.Abstractions.Model;

public class VkUser
{
	public int Id { get; set; }
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string Photo { get; set; } = string.Empty;
}