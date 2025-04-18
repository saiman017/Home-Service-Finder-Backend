using Home_Service_Finder.Data;
using Home_Service_Finder.Roles.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Home_Service_Finder.Roles
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        private readonly AppDbContext _dbContext;
        public RoleRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Role> FindByNameAsync(string name)
        {
            var entity = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Name == name && x.IsDeleted == false);
            return entity;
            
        }
    }
}
