namespace Energizet.Box.Entities;

public class Info
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public VkUser? VkUser { get; set; }
}