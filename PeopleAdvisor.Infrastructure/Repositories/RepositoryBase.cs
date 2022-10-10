using Microsoft.EntityFrameworkCore;
using PeopleAdvisor.Domain.Interfaces;
using PeopleAdvisor.Infrastructure.Data;

namespace PeopleAdvisor.Infrastructure.Repositories;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{

    private readonly DbSet<T> _dbSet;
    private readonly ApplicationContext _applicationContext;

    public RepositoryBase(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
        _dbSet = applicationContext.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _applicationContext.Entry(entity).State = EntityState.Detached;
        }

        return entity;
    }

    public async Task<IEnumerable<T?>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _applicationContext.Set<T>().AddAsync(entity);
        await _applicationContext.SaveChangesAsync();

        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _applicationContext.Entry(entity).State = EntityState.Modified;
        return _applicationContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _applicationContext.Entry(entity).State = EntityState.Deleted;
            await _applicationContext.SaveChangesAsync();
        }
    }
}