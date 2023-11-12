using Energizet.Box.Db.Abstractions.Model;

namespace Energizet.Box.Db.Abstractions;

public interface IDbProvider
{
	Task<Guid> NewAsync(string contentType, CancellationToken token);

	Task SaveAsync(
		Guid id, string title, int vkUserId, CancellationToken token
	);

	Task<FileDb> FindAsync(Guid id, CancellationToken token);
}