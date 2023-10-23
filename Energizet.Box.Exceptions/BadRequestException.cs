namespace Energizet.Box.Exceptions;

public class BadRequestException : Exception
{
	public BadRequestException(string message) : base(message)
	{
	}
}