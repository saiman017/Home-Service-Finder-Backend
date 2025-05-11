using System.Diagnostics;
using Home_Service_Finder.Data;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Ratings.Contracts;
using Home_Service_Finder.Ratings.Dtos;

namespace Home_Service_Finder.Ratings
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _dbContext;
        public RatingService(IUnitOfWork unitOfWork) {

            _dbContext = unitOfWork;
        }

        public async Task<APIResponse> AddRatingAsync( RatingRequestDto dto)
        {
            // Prevent duplicate
            var provider = await _dbContext.ServiceProviders
                                  .GetByIdAsync(dto.ServiceProviderId);
            if (provider == null)
                return ResponseHandler.GetBadRequestResponse(
                    $"ServiceProviderId {dto.ServiceProviderId} not found");

            // 2) (Optional) Validate requestId similarly…
            if (dto.ServiceRequestId == Guid.Empty)
                dto.ServiceRequestId = null;
            else if (dto.ServiceRequestId.HasValue)
            {
                var req = await _dbContext.ServiceRequests
                                          .GetByIdAsync(dto.ServiceRequestId.Value);
                if (req == null)
                    return ResponseHandler.GetBadRequestResponse(
                        $"ServiceRequestId {dto.ServiceRequestId} not found");
            }


            var rating = new Rating
            {
                CustomerId = dto.CustomerId,
                ServiceProviderId = dto.ServiceProviderId,
                ServiceRequestId = dto.ServiceRequestId,
                Value = dto.Value,
                Comments = dto.Comments
            };

            await _dbContext.Ratings.AddAsync(rating);
            string result = await _dbContext.SaveChangesAsync();
            Console.WriteLine(result);

            var resp = new RatingResponseDto
            {
                Id = rating.Id,
                CustomerId = rating.CustomerId,
                ServiceProviderId = rating.ServiceProviderId,
                Value = rating.Value,
                Comments = rating.Comments,
                CreatedAt = rating.CreatedAt
            };
            return ResponseHandler.GetSuccessResponse(resp, "Rating submitted successfully");
        }

        public async Task<APIResponse> GetRatingsForProviderAsync(Guid providerId)
        {
            var list = await _dbContext.Ratings.GetByProviderAsync(providerId);
            var dtos = list.Select(r => new RatingResponseDto
            {
                Id = r.Id,
                CustomerId = r.CustomerId,
                ServiceProviderId = r.ServiceProviderId,
                Value = r.Value,
                Comments = r.Comments,
                CreatedAt = r.CreatedAt
            }).ToList();
            return ResponseHandler.GetSuccessResponse(dtos);
        }

        public async Task<APIResponse> GetRatingStatsAsync(Guid providerId)
        {
            var ratings = await _dbContext.Ratings.GetByProviderAsync(providerId);

            var count = ratings.Count();
            var sum = ratings.Sum(r => r.Value);
            var avg = count > 0 ? (double)sum / count : 0d;

            var dto = new RatingStatsDto
            {
                ServiceProviderId = providerId,
                Count = count,
                Sum = sum,
                Average = avg
            };

            return ResponseHandler.GetSuccessResponse(dto);
        }


    }
}
