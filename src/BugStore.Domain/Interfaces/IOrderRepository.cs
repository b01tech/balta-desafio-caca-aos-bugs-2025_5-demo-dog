using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync(int page, int pageSize);
        Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
        Task<Order?> GetWithOrderLinesAsync(Guid id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Guid id);
    }
}