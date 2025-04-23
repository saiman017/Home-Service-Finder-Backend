using Home_Service_Finder.Data;
using Home_Service_Finder.RequestServices.ServiceOffers.Contracts;

namespace Home_Service_Finder.RequestServices.ServiceOffers
{
    public class ServiceOfferRepository : GenericRepository<ServiceOffer>, IServiceOfferRepository
    {
        public ServiceOfferRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
