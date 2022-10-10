namespace PeopleAdvisor.Domain.Interfaces;

public interface IRepositoryBase<T> where T : class
{
    Task<IEnumerable<T?>> GetAllAsync();

    Task<T?> GetByIdAsync(int id);

    Task<T> AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task RemoveAsync(int id);
}