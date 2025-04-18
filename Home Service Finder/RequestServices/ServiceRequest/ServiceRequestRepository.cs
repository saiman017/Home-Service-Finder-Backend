using Home_Service_Finder.Data;
using Home_Service_Finder.RequestServices.ServiceRequest.Contracts;

namespace Home_Service_Finder.RequestServices.ServiceRequest
{
    public class ServiceRequestRepository : GenericRepository<ServiceRequest>, IServiceRequestRepositpry
    {
        public ServiceRequestRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
