using BookLendingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookLendingSystem.Application.Interfaces
{
    public interface IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
    {
        Task<TEntity> GetByIdAsync(TId id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
