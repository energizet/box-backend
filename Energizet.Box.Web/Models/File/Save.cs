namespace Energizet.Box.Web.Models.File;

public class SaveRequest
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string VkLink { get; set; } = string.Empty;
}