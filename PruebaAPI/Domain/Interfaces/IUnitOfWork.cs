namespace PruebaAPI.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IMarcaAutoRepository MarcasAutos { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
