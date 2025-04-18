using Home_Service_Finder.Data.Contracts;

namespace Home_Service_Finder.Locations.Contracts
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        Task<Location> GetByUserIdAsync(Guid userId);
        //Task UpdatesAsync(Location location);

    }
}
