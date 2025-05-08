using Home_Service_Finder.Data.Contracts;

namespace Home_Service_Finder.Ratings.Contracts
{
    public interface IRatingRepository : IGenericRepository<Rating>
    {
        Task<bool> HasCustomerRatedAsync(Guid customerId, Guid serviceProviderId, Guid? serviceRequestId);
        Task<IEnumerable<Rating>> GetByProviderAsync(Guid serviceProviderId);
    }
}
