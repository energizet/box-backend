using Energizet.Box.Db.Abstractions;
using Energizet.Box.Entities;
using Energizet.Box.Exceptions;
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

	public async Task<Guid> CreateAsync(Stream streamFile, string contentType,
		CancellationToken token)
	{
		var id = await _dbProvider.NewAsync(contentType, token);
		await _storeProvider.NewAsync(id, streamFile, token);

		return id;
	}

	public async Task SaveAsync(Guid id, string title, string vkLink, CancellationToken token)
	{
		var vkId = vkLink.Split("/").Last();
		var vkUser = await _vkProvider.GetVkUser(vkId, token);

		await _dbProvider.SaveAsync(id, title, vkUser.Id, token);
		await _storeProvider.SaveAsync(id, token);
	}

	public async Task<Info> InfoAsync(Guid id, CancellationToken token)
	{
		var fileDb = await _dbProvider.FindAsync(id, token);
		var vkUser = await _vkProvider.GetVkUser(fileDb.VkUserId, token);

		return new()
		{
			Id = fileDb.Id,
			Title = fileDb.Title,
			VkUser = vkUser,
		};
	}

	public async Task<File> GetAsync(Guid id, string vkUserId, CancellationToken token)
	{
		var fileDb = await _dbProvider.FindAsync(id, token);
		if (fileDb.VkUserId != vkUserId)
		{
			throw new ForbiddenException($"vkUserId({vkUserId}) forbidden");
		}

		var fileStream = await _storeProvider.GetAsync(id, token);

		return new()
		{
			ContentType = fileDb.ContentType,
			Stream = fileStream,
		};
	}
}

public class File
{
	public string ContentType { get; set; } = string.Empty;
	public Stream Stream { get; set; } = Stream.Null;
}