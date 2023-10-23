namespace Energizet.Box.Db.Model;

public class FileDb
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string VkUserId { get; set; } = string.Empty;
}