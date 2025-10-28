namespace BugStore.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}