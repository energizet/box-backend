namespace Energizet.Box.Store.Abstraction;

public interface IStoreProvider
{
	Task NewAsync(Guid id, Stream stream, CancellationToken token);
	Task SaveAsync(Guid id, CancellationToken token);
}