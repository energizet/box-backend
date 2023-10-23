using Energizet.Box.Entities;

namespace Energizet.Box.Vk.Abstractions;

public interface IVkProvider
{
	Task<VkUser> GetVkUser(string vkId, CancellationToken token);
}