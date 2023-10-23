using Energizet.Box.Db.Abstractions.Model;

namespace Energizet.Box.Db.Abstractions;

public interface IDbProvider
{
	Task SaveAsync(
		Guid id, string title, int vkUserId, CancellationToken token
	);

	Task<FileDb> Find(Guid id, CancellationToken token);
}