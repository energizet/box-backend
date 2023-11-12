using System.Text.Json;
using Energizet.Box.Db.Abstractions;
using Energizet.Box.Db.Abstractions.Model;
using Energizet.Box.Db.Entity;
using Energizet.Box.Exceptions;

namespace Energizet.Box.Db;

public sealed class DbProvider : IDbProvider
{
	private static Dictionary<Guid, FileEntity> s_fileDb = new();

	static DbProvider()
	{
		LoadDbAsync().Wait();
	}

	public async Task<Guid> NewAsync(string contentType, CancellationToken token)
	{
		var id = Guid.NewGuid();
		s_fileDb[id] = new()
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
		if (s_fileDb.ContainsKey(id) == false)
		{
			throw new NotFoundException($"id({id}) not found");
		}

		s_fileDb[id].Title = title;
		s_fileDb[id].VkUserId = vkUserId;
		s_fileDb[id].IsSaved = true;

		await SaveDbAsync();
	}

	public Task<FileDb> FindAsync(Guid id, CancellationToken token)
	{
		if (s_fileDb.ContainsKey(id) == false || s_fileDb[id].IsSaved == false)
		{
			throw new NotFoundException($"id({id}) not found");
		}

		var file = s_fileDb[id];

		var fileDb = new FileDb
		{
			Id = file.Id,
			Title = file.Title!,
			VkUserId = file.VkUserId.ToString()!,
			ContentType = file.ContentType,
		};

		return Task.FromResult(fileDb);
	}

	private static async Task LoadDbAsync()
	{
		await CreateDbAsync();
		var json = await File.ReadAllTextAsync("./db/files.db");
		s_fileDb = JsonSerializer.Deserialize<Dictionary<Guid, FileEntity>>(json)!;
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
		await File.WriteAllTextAsync("./db/files.db", JsonSerializer.Serialize(s_fileDb));
	}
}