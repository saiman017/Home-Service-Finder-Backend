using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Roles;

namespace Home_Service_Finder.ServicesList.Contracts
{
    public interface IServiceListRepository : IGenericRepository<ServiceList>
    {
        Task<ServiceList> FindByNameAsync(string name);
        Task<IEnumerable<ServiceList>> GetServiceListByCategoryId(Guid id);


    }
}
