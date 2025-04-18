using Home_Service_Finder.Data;
using Home_Service_Finder.Users.ServiceProvider.Contracts;

namespace Home_Service_Finder.Users.ServiceProvider
{
    public class ServiceProviderRepository : GenericRepository<ServiceProvider>, IServiceProviderRepository
    {
        //private readonly AppDbContext _dbContext;

        
        public ServiceProviderRepository(AppDbContext dbContext) : base(dbContext)
        {
            //_dbContext = dbContext;
        }

        //public Task<IEnumerable<ServiceProvider>> GetActiveByServiceAsync(Guid serviceId)
        //{
        //   return 

        //}

        //public Task<IEnumerable<ServiceProvider>> GetAllByServiceCategoryAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<ServiceProvider>> GetByVerificationStatusAsync(bool isVerified)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
