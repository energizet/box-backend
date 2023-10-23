using Energizet.Box.Vk.Abstractions.Model;

namespace Energizet.Box.Vk.Abstractions;

public interface IVkProvider
{
	Task<VkUser> GetVkUser(string vkId, CancellationToken token);
}