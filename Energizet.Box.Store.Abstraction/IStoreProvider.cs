namespace Energizet.Box.Store.Abstraction;

public interface IStoreProvider
{
	Task<Stream> NewAsync(Guid id);
	Task SaveAsync(Guid id);
}