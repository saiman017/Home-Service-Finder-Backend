using Home_Service_Finder.Data;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.RequestServices.ServiceRequest.Contracts;

namespace Home_Service_Finder.RequestServices.ServiceRequest
{
    public class ServiceRequestImageRepository : GenericRepository<ServiceRequestImage>, IServiceRequestImageRepository
    {
        public ServiceRequestImageRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
