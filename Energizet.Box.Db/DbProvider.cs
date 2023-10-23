using Energizet.Box.Db.Abstractions;
using Energizet.Box.Db.Abstractions.Model;
using Energizet.Box.Exceptions;

namespace Energizet.Box.Db;

public sealed class DbProvider : IDbProvider
{
	public async Task SaveAsync(
		Guid id, string title, int vkUserId, CancellationToken token
	)
	{
		await CreateDbAsync();

		await File.AppendAllLinesAsync(
			"./db/files.txt",
			new[]
			{
				$"{id}|{title}|{vkUserId}",
			},
			token
		);
	}

	public async Task<FileDb> Find(Guid id, CancellationToken token)
	{
		await CreateDbAsync();

		var files = (await File.ReadAllLinesAsync("./db/files.txt", token))
			.Select(line => line.Split("|"))
			.ToList();

		var file = files.FirstOrDefault(line => line[0] == id.ToString());

		if (file == null)
		{
			throw new NotFoundException($"id({id}) not found");
		}

		var fileDb = new FileDb
		{
			Id = Guid.Parse(file[0]),
			Title = file[1],
			VkUserId = file[2],
		};

		return fileDb;
	}

	private static async Task CreateDbAsync()
	{
		if (Directory.Exists("./db") == false)
		{
			Directory.CreateDirectory("./db");
		}

		if (File.Exists("./db/files.txt") == false)
		{
			await using var tmpFile = File.Create("./db/files.txt");
		}
	}
}