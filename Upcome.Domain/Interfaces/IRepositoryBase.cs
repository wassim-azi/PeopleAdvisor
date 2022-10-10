﻿namespace Upcome.Domain.Interfaces;

public interface IRepositoryBase<T> where T : class
{
    Task<T?> GetByIdAsync(int id);

    Task<IEnumerable<T?>> GetAllAsync();

    Task<T> AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task RemoveAsync(int id);
}