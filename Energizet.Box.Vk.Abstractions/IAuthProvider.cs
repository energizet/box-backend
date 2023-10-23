namespace Energizet.Box.Vk.Abstractions;

public interface IAuthProvider
{
	Task<bool> VerifyHashAsync(int vkUserId, string hash, CancellationToken token);
}