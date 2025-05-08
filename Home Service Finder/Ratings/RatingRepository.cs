using Home_Service_Finder.Data;
using Home_Service_Finder.Ratings.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Home_Service_Finder.Ratings
{
    public class RatingRepository : GenericRepository<Rating>, IRatingRepository
    {
        private readonly AppDbContext _db;
        public RatingRepository(AppDbContext dbContext) : base(dbContext)
        {
            _db = dbContext;
        }

        public async Task<bool> HasCustomerRatedAsync(Guid customerId, Guid serviceProviderId, Guid? serviceRequestId)
        {
            return await _db.Ratings.AnyAsync(r =>
                r.CustomerId == customerId
                && r.ServiceProviderId == serviceProviderId
                && (serviceRequestId == null || r.ServiceRequestId == serviceRequestId));
        }

        public async Task<IEnumerable<Rating>> GetByProviderAsync(Guid serviceProviderId)
        {
            return await _db.Ratings
                .Where(r => r.ServiceProviderId == serviceProviderId)
                .ToListAsync();
        }
    }
}
