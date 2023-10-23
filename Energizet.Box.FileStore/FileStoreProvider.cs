using Energizet.Box.Store.Abstraction;

namespace Energizet.Box.FileStore;

public sealed class FileStoreProvider : IStoreProvider
{
	private readonly string _tempDir;
	private readonly string _storeDir;

	public FileStoreProvider(string tempDir, string storeDir)
	{
		_tempDir = tempDir;
		_storeDir = storeDir;
	}

	public async Task NewAsync(Guid id, Stream stream, CancellationToken token)
	{
		await using var newFile = await CreateAsync(_tempDir, id);
		await stream.CopyToAsync(newFile, token);
	}

	public Task SaveAsync(Guid id, CancellationToken token)
	{
		var sourceFileName = CreatePath(_tempDir, id);
		var destFileName = CreatePath(_storeDir, id);

		File.Copy(sourceFileName, destFileName);
		File.Delete(sourceFileName);

		return Task.CompletedTask;
	}

	private Task<Stream> CreateAsync(string dir, Guid id)
	{
		return Task.FromResult((Stream)File.Create(CreatePath(dir, id)));
	}

	private static string CreatePath(string dir, Guid id)
	{
		if (Directory.Exists(dir) == false)
		{
			Directory.CreateDirectory(dir);
		}

		return Path.Combine(dir, id.ToString());
	}
}