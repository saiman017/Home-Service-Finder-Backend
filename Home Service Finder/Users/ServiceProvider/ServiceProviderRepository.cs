using Home_Service_Finder.Data;
using Home_Service_Finder.Users.ServiceProvider.Contracts;

namespace Home_Service_Finder.Users.ServiceProvider
{
    public class ServiceProviderRepository : GenericRepository<ServiceProvider>, IServiceProviderRepository
    {
        
        public ServiceProviderRepository(AppDbContext dbContext) : base(dbContext)
        {
           
        }

    }
}
