using System.Linq.Expressions;

namespace Alumni.Domain.Interfaces;

/// <summary>
/// Tüm repository'ler için generic CRUD arayüzü.
/// SIS ve diğer veri kaynaklarına kolay geçiş sağlar.
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}

