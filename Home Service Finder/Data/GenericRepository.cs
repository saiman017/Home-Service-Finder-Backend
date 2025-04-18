using Home_Service_Finder.Data.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Home_Service_Finder.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
            
        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<T?> DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if(entity == null)
            {
                return null;
            }
            _dbSet.Remove(entity);
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if( entity == null)
            {
                return null;
            }
            return entity;
        }

        public T UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _dbSet.Entry(entity).State = EntityState.Modified;
            return entity;

        }
    }
}
