using Home_Service_Finder.Data.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Home_Service_Finder.Users.ServiceProvider.Contracts
{
    public interface IServiceProviderRepository : IGenericRepository<ServiceProvider>
    {
        //Task<IEnumerable<ServiceProvider>> GetAllByServiceCategoryAsync();
        //Task<IEnumerable<ServiceProvider>> GetActiveByServiceAsync(Guid serviceId);
        //    // More flexible than hardcoded queries

        ////    public async Task<IEnumerable<ServiceProvider>> GetActiveByServiceAsync(Guid serviceId)
        ////{
        ////    return await _context.ServiceProviders
        ////        .Include(sp => sp.User)          // Load user details
        ////        .Include(sp => sp.Service)       // Load service details
        ////        .Where(sp => sp.ServiceId == serviceId &&
        ////                    sp.IsActive)         // Only active providers
        ////        .OrderBy(sp => sp.Experience)    // Optional: Sort by experience
        ////        .ToListAsync();
        ////}
        //Task<IEnumerable<ServiceProvider>> GetByVerificationStatusAsync(bool isVerified);



    }
}
