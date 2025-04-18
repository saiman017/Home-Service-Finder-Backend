using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Email;
using Home_Service_Finder.Email.Contracts;
using Home_Service_Finder.Locations;
using Home_Service_Finder.Locations.Contracts;
using Home_Service_Finder.RequestServices.ServiceRequest;
using Home_Service_Finder.RequestServices.ServiceRequest.Contracts;
using Home_Service_Finder.Roles;
using Home_Service_Finder.Roles.Contracts;
using Home_Service_Finder.ServiceCategories;
using Home_Service_Finder.ServiceCategories.Contracts;
using Home_Service_Finder.ServicesList;
using Home_Service_Finder.ServicesList.Contracts;
using Home_Service_Finder.Users;
using Home_Service_Finder.Users.ServiceProvider;
using Home_Service_Finder.Users.ServiceProvider.Contracts;
using Home_Service_Finder.Users.UserDetails;
using Home_Service_Finder.Users.UserDetails.Contracts;
//using Home_Service_Finder.Users.UserRoles.Contracts;
using Home_Service_Finder.Users.Users;
using Home_Service_Finder.Users.Users.Contracts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Home_Service_Finder.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private IDbContextTransaction _transaction;
        public IUserRepository Users { get; private set; }
        public IRoleRepository Roles { get; private set; }
        public  IUserDetailRepository UserDetails { get; private set; }

        public IServiceProviderRepository ServiceProviders { get; private set; }

        public IServiceCategoryRepository ServiceCategories { get; private set; }

        public IEmailOTPRepository Emails{ get; private set; }

        public ILocationRepository Locations { get; private set; }
        public IServiceRequestRepositpry ServiceRequests { get; private set; }
        
        public IServiceRequestServiceListRepsoitory ServiceRequestServiceLists { get; private set; }
         


        public IServiceListRepository ServiceLists { get; private set; }


        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            Users = new UserRepository(_dbContext);
            Roles = new RoleRepository(_dbContext);
            UserDetails = new UserDetailRepository(_dbContext);
            ServiceProviders = new ServiceProviderRepository(_dbContext);
            ServiceCategories = new ServiceCategoryRepository(_dbContext);
            Emails = new EmailOTPRepository(_dbContext);
            Locations = new LocationRepository(_dbContext);
            ServiceLists = new ServiceListRepository(_dbContext);
            ServiceRequests = new ServiceRequestRepository(_dbContext);
            ServiceRequestServiceLists = new ServiceRequestServiceListRepsoitory(_dbContext);

        }
        public Task BeginTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task CommitAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public Task RollBackAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string> SaveChangesAsync()
        {
            try
            {
                var result = await _dbContext.SaveChangesAsync();

                if (result > 0)
                    return "Successfully saved!";
                else if (result == 0)
                    return "No changes were saved";
                else
                    return "An error encountered while performing save operation";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
                throw;
            }

        }
    }
}
