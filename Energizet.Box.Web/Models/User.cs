namespace Energizet.Box.Web.Models;

public class User
{
	public static readonly User Empty = new();

	public int Uid { get; set; }
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string Photo { get; set; } = string.Empty;
	public string PhotoRec { get; set; } = string.Empty;
	public string Hash { get; set; } = string.Empty;
}