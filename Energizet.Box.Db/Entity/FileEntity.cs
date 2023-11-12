namespace Energizet.Box.Db.Entity;

public class FileEntity
{
	public Guid Id { get; set; }
	public string? Title { get; set; }
	public int? VkUserId { get; set; }
	public string ContentType { get; set; } = string.Empty;

	public bool IsSaved { get; set; }
}