using Home_Service_Finder.Email.Contracts;
using Home_Service_Finder.Locations.Contracts;
using Home_Service_Finder.RequestServices.ServiceOffers.Contracts;
using Home_Service_Finder.RequestServices.ServiceRequest.Contracts;
using Home_Service_Finder.Roles.Contracts;
using Home_Service_Finder.ServiceCategories.Contracts;
using Home_Service_Finder.ServicesList.Contracts;
using Home_Service_Finder.Users.ServiceProvider.Contracts;
using Home_Service_Finder.Users.UserDetails.Contracts;
//using Home_Service_Finder.Users.UserRoles.Contracts;
using Home_Service_Finder.Users.Users.Contracts;

namespace Home_Service_Finder.Data.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IServiceProviderRepository  ServiceProviders {get;}

        IServiceCategoryRepository ServiceCategories { get; }

        IUserDetailRepository UserDetails { get;  }

        ILocationRepository Locations { get;  }

        IServiceListRepository ServiceLists { get; }
        IServiceRequestRepositpry ServiceRequests { get;  }

        IServiceRequestServiceListRepsoitory ServiceRequestServiceLists { get;  }

        IServiceOfferRepository ServiceOffers{ get; }

        IEmailOTPRepository Emails { get; }
        

        Task<string> SaveChangesAsync();

        Task BeginTransactionAsync();

        Task CommitAsync();

        Task RollBackAsync();


    }
}
