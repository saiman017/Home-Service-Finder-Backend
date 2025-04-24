using Home_Service_Finder.Data;
using Home_Service_Finder.RequestServices.ServiceOffers.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Home_Service_Finder.RequestServices.ServiceOffers
{
    public class ServiceOfferRepository : GenericRepository<ServiceOffer>, IServiceOfferRepository
    {
        private readonly AppDbContext _dbContext; 

        public ServiceOfferRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceOffer?> GetOfferWithRequestAsync(Guid offerId)
        {
            return await _dbContext.ServiceOffers
                .Include(o => o.ServiceRequest)
                .FirstOrDefaultAsync(o => o.Id == offerId);
        }


    }
}
