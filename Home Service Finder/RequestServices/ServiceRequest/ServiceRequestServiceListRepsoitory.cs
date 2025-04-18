using Home_Service_Finder.Data;
using Home_Service_Finder.RequestServices.ServiceRequest.Contracts;

namespace Home_Service_Finder.RequestServices.ServiceRequest
{
    public class ServiceRequestServiceListRepsoitory : GenericRepository<ServiceRequestServiceList>, IServiceRequestServiceListRepsoitory
    {
        public ServiceRequestServiceListRepsoitory(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
