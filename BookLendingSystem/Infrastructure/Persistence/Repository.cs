using BookLendingSystem.Application.Interfaces;
using BookLendingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookLendingSystem.Infrastructure.Persistence
{
    public class Repository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(TId id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
