using Home_Service_Finder.Data.Contracts;

namespace Home_Service_Finder.RequestServices.ServiceOffers.Contracts
{
    public interface IServiceOfferRepository : IGenericRepository<ServiceOffer>
    {
        Task<ServiceOffer?> GetOfferWithRequestAsync(Guid offerId);
    }
}
