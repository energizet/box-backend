using Energizet.Box.Db.Abstractions;
using Energizet.Box.Entities;
using Energizet.Box.Store.Abstraction;
using Energizet.Box.Vk.Abstractions;

namespace Energizet.Box.Core;

public sealed class FileCases
{
	private readonly IStoreProvider _storeProvider;
	private readonly IVkProvider _vkProvider;
	private readonly IDbProvider _dbProvider;

	public FileCases(
		IStoreProvider storeProvider, IVkProvider vkProvider, IDbProvider dbProvider
	)
	{
		_storeProvider = storeProvider;
		_vkProvider = vkProvider;
		_dbProvider = dbProvider;
	}

	public async Task<Guid> CreateAsync(Stream streamFile, CancellationToken token)
	{
		var id = await _dbProvider.NewAsync(token);
		await _storeProvider.NewAsync(id, streamFile, token);

		return id;
	}

	public async Task SaveAsync(Guid id, string title, string vkLink, CancellationToken token)
	{
		var vkId = vkLink.Split("/").Last();
		var vkUser = await _vkProvider.GetVkUser(vkId, token);

		await _storeProvider.SaveAsync(id, token);
		await _dbProvider.SaveAsync(id, title, vkUser.Id, token);
	}

	public async Task<Info> InfoAsync(Guid id, CancellationToken token)
	{
		var fileDb = await _dbProvider.Find(id, token);
		var vkUser = await _vkProvider.GetVkUser(fileDb.VkUserId, token);
		
		return new()
		{
			Id = fileDb.Id,
			Title = fileDb.Title,
			VkUser = vkUser,
		};
	}
}