using Home_Service_Finder.Ratings.Contracts;
using Home_Service_Finder.Ratings.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Home_Service_Finder.Ratings
{
    [Route("api/rating")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _svc;
        public RatingController(IRatingService svc) => _svc = svc;

        [HttpPost]
        public async Task<APIResponse> Add([FromBody] RatingRequestDto dto)
        {
           
           
            return await _svc.AddRatingAsync( dto);
        }

        [HttpGet("provider/{providerId:guid}")]
        public async Task<APIResponse> GetByProvider(Guid providerId)
            => await _svc.GetRatingsForProviderAsync(providerId);

        [HttpGet("provider/{providerId:guid}/stats")]
        public async Task<APIResponse> GetStats(Guid providerId)
           => await _svc.GetRatingStatsAsync(providerId);
    }


}
