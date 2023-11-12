namespace Energizet.Box.Jwt.Abstractions;

public interface IJwtProvider
{
	string CreateToken(int vkUserId);
}