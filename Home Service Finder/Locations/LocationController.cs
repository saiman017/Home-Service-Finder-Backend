using Home_Service_Finder.Locations.Contracts;
using Home_Service_Finder.Locations.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Home_Service_Finder.Locations
{
    [Route("api/location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("{userId}")]
        public async Task<APIResponse> GetLocationAsync(Guid userId)
        {
            var apiResponse = await _locationService.GetLocationAsync(userId);
            return apiResponse;
        }

        [HttpPost]
        public async Task<APIResponse> SaveLocationAsync( [FromBody] LocationRequestDto locationRequestDto)
        {
            if (locationRequestDto == null)
                return ResponseHandler.GetBadRequestResponse("Invalid location data");

            return await _locationService.SaveLocationAsync( locationRequestDto);
        }

        // ✅ Unified PUT method for both create (if not exists) and update
        [HttpPut("{userId}")]
        public async Task<APIResponse> UpsertLocationAsync(Guid userId, [FromBody] LocationRequestDto locationRequestDto)
        {
            if (locationRequestDto == null)
                return ResponseHandler.GetBadRequestResponse("Invalid location data");

            return await _locationService.UpdateLocationAsync(userId, locationRequestDto);
        }
    }
}
