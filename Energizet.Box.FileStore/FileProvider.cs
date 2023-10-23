namespace Energizet.Box.FileStore;

public sealed class FileProvider
{
	private readonly string _tempDir;
	private readonly string _storeDir;

	public FileProvider(string tempDir, string storeDir)
	{
		_tempDir = tempDir;
		_storeDir = storeDir;
	}

	public Task<Stream> NewAsync(Guid id)
	{
		return CreateAsync(_tempDir, id);
	}

	public Task SaveAsync(Guid id)
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