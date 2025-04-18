using Home_Service_Finder.Data;
using Home_Service_Finder.Locations.Contracts;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace Home_Service_Finder.Locations
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        private readonly AppDbContext _dbContext;
        public LocationRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Location> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.Locations.FirstOrDefaultAsync(l => l.UserId == userId);
        }

       
    }
}
