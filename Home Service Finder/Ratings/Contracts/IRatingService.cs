using Home_Service_Finder.Ratings.Dtos;

namespace Home_Service_Finder.Ratings.Contracts
{
    public interface IRatingService
    {
        Task<APIResponse> AddRatingAsync( RatingRequestDto dto);
        Task<APIResponse> GetRatingsForProviderAsync(Guid providerId);
        Task<APIResponse> GetRatingStatsAsync(Guid providerId);   //
    }
}
