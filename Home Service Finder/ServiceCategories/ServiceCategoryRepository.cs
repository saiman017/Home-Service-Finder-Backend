using Home_Service_Finder.Data;
using Home_Service_Finder.ServiceCategories.Contracts;
using Home_Service_Finder.Users.ServiceProvider.Contracts;
using Home_Service_Finder.Users.Users;
using Microsoft.EntityFrameworkCore;

namespace Home_Service_Finder.ServiceCategories
{
    public class ServiceCategoryRepository : GenericRepository<ServiceCategory>, IServiceCategoryRepository
    {
        internal DbSet<ServiceCategory> _dbSet;

        public ServiceCategoryRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbSet = dbContext.Set<ServiceCategory>();
        }

        public async Task<ServiceCategory> GetByServiceCategoryName(string name)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.Name == name );
            return entity;
        }
    }
}
