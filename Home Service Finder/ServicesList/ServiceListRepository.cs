using Home_Service_Finder.Data;
using Home_Service_Finder.Roles;
using Home_Service_Finder.ServicesList.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Home_Service_Finder.ServicesList
{
    public class ServiceListRepository : GenericRepository<ServiceList>, IServiceListRepository
    {
        private readonly AppDbContext _dbContext;

        public ServiceListRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

       

        public async Task<ServiceList> FindByNameAsync(string name)
        {
            var entity = await _dbContext.ServiceLists.FirstOrDefaultAsync(x => x.Name == name);
            return entity;

        }

        public async Task<IEnumerable<ServiceList>> GetServiceListByCategoryId(Guid id)
        {
            return await _dbContext.ServiceLists
                .Where(x => x.ServiceCategoryId == id)
                .ToListAsync();
        }
    }
}
