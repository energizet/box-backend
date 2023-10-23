namespace Energizet.Box.Db.Abstractions.Model;

public class FileDb
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string VkUserId { get; set; } = string.Empty;
}