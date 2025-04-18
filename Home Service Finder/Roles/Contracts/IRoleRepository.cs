using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Roles;

namespace Home_Service_Finder.Roles.Contracts
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> FindByNameAsync(string name);
    }
}
