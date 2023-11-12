using System.Text.Json;
using Energizet.Box.Db.Abstractions;
using Energizet.Box.Db.Abstractions.Model;
using Energizet.Box.Db.Entity;
using Energizet.Box.Exceptions;

namespace Energizet.Box.Db;

public sealed class DbProvider : IDbProvider
{
	private static readonly Dictionary<Guid, FileEntity> FileDb;

	static DbProvider()
	{
		var db = LoadDbAsync();
		db.Wait();
		FileDb = db.Result;
	}

	public async Task<Guid> NewAsync(string contentType, CancellationToken token)
	{
		var id = Guid.NewGuid();
		FileDb[id] = new()
		{
			Id = id,
			ContentType = contentType,
		};
		await SaveDbAsync();
		return id;
	}

	public async Task SaveAsync(
		Guid id, string title, int vkUserId, CancellationToken token
	)
	{
		if (FileDb.ContainsKey(id) == false)
		{
			throw new NotFoundException($"id({id}) not found");
		}

		FileDb[id].Title = title;
		FileDb[id].VkUserId = vkUserId;
		FileDb[id].IsSaved = true;

		await SaveDbAsync();
	}

	public Task<FileDb> FindAsync(Guid id, CancellationToken token)
	{
		if (FileDb.ContainsKey(id) == false || FileDb[id].IsSaved == false)
		{
			throw new NotFoundException($"id({id}) not found");
		}

		var file = FileDb[id];

		var fileDb = new FileDb
		{
			Id = file.Id,
			Title = file.Title!,
			VkUserId = file.VkUserId.ToString()!,
			ContentType = file.ContentType,
		};

		return Task.FromResult(fileDb);
	}

	private static async Task<Dictionary<Guid, FileEntity>> LoadDbAsync()
	{
		await CreateDbAsync();
		var json = await File.ReadAllTextAsync("./db/files.db");
		return JsonSerializer.Deserialize<Dictionary<Guid, FileEntity>>(json)!;
	}

	private static async Task CreateDbAsync()
	{
		if (Directory.Exists("./db") == false)
		{
			Directory.CreateDirectory("./db");
		}

		if (File.Exists("./db/files.db") == false)
		{
			await File.Create("./db/files.db").DisposeAsync();
			await SaveDbAsync();
		}
	}

	private static async Task SaveDbAsync()
	{
		await File.WriteAllTextAsync("./db/files.db", JsonSerializer.Serialize(FileDb));
	}
}