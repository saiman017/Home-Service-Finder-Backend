using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Locations.Contracts;
using Home_Service_Finder.Locations.Dtos;
using Home_Service_Finder.Locations;
using Home_Service_Finder;

public class LocationService : ILocationService
{
    private readonly IUnitOfWork _dbContext;

    public LocationService(IUnitOfWork unitOfWork)
    {
        _dbContext = unitOfWork;
    }

    public async Task<APIResponse> GetLocationAsync(Guid userId)
    {
        try
        {
            var location = await _dbContext.Locations.GetByUserIdAsync(userId);

            if (location == null)
                return ResponseHandler.GetNotFoundResponse("Location not found");

            var response = new LocationResponseDTO
            {
                UserId = location.UserId,
                Address = location.Address,
                City = location.City,
                PostalCode = location.PostalCode,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                UpdatedAt = location.UpdatedAt
            };

            return ResponseHandler.GetSuccessResponse(response, "Location retrieved successfully");
        }
        catch (Exception)
        {
            return ResponseHandler.GetBadRequestResponse("Failed to retrieve location");
        }
    }

    public async Task<APIResponse> SaveLocationAsync( LocationRequestDto locationRequestDto)
    {
        try
        {
            if (locationRequestDto.UserId == Guid.Empty || locationRequestDto == null)
                return ResponseHandler.GetBadRequestResponse("Invalid user ID or data");

            var user = await _dbContext.Users.GetByIdAsync(locationRequestDto.UserId);
            if (user == null)
                return ResponseHandler.GetNotFoundResponse("User not found");

            if (string.IsNullOrEmpty(locationRequestDto.Address) ||
                locationRequestDto.Latitude == 0 ||
                locationRequestDto.Longitude == 0)
                return ResponseHandler.GetBadRequestResponse("Address, latitude, and longitude are required");

            var existing = await _dbContext.Locations.GetByUserIdAsync(locationRequestDto.UserId);
            if (existing != null)
                return ResponseHandler.GetBadRequestResponse("Location already exists, use update method");

            var location = new Location
            {
                UserId = locationRequestDto.UserId,
                Address = locationRequestDto.Address,
                City = locationRequestDto.City ?? string.Empty,
                PostalCode = locationRequestDto.PostalCode ?? string.Empty,
                Latitude = locationRequestDto.Latitude,
                Longitude = locationRequestDto.Longitude,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbContext.Locations.AddAsync(location);
            string result = await _dbContext.SaveChangesAsync();

            Console.WriteLine($"SaveChangesAsync result: {result}");

            var response = new LocationResponseDTO
            {
                UserId = location.UserId,
                Address = location.Address,
                City = location.City,
                PostalCode = location.PostalCode,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                UpdatedAt = location.UpdatedAt
            };

            return result == "Successfully saved!"
                ? ResponseHandler.GetSuccessResponse(response, result)
                : ResponseHandler.GetBadRequestResponse($"Failed to save: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in SaveLocationAsync: {ex}");
            return ResponseHandler.GetBadRequestResponse($"Exception: {ex.Message}");
        }
    }


    public async Task<APIResponse> UpdateLocationAsync(Guid userId, LocationRequestDto dto)
    {
        if (userId == Guid.Empty || dto == null)
            return ResponseHandler.GetBadRequestResponse("Invalid user ID or data");

        if (string.IsNullOrEmpty(dto.Address) || dto.Latitude == 0 || dto.Longitude == 0)
            return ResponseHandler.GetBadRequestResponse("Address, latitude, and longitude are required");

        var location = await _dbContext.Locations.GetByUserIdAsync(userId);

        if (location == null)
        {
            // Save as new location
            location = new Location
            {
                UserId = userId,
                Address = dto.Address,
                City = dto.City ?? string.Empty,
                PostalCode = dto.PostalCode ?? string.Empty,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbContext.Locations.AddAsync(location);
        }
        else
        {
            location.Address = dto.Address;
            location.City = dto.City ?? location.City;
            location.PostalCode = dto.PostalCode ?? location.PostalCode;
            location.Latitude = dto.Latitude;
            location.Longitude = dto.Longitude;
            location.UpdatedAt = DateTime.UtcNow;

            _dbContext.Locations.UpdateAsync(location);
        }

        string result = await _dbContext.SaveChangesAsync();

        var response = new LocationResponseDTO
        {
            UserId = userId,
            Address = location.Address,
            City = location.City,
            PostalCode = location.PostalCode,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            UpdatedAt = location.UpdatedAt
        };

        return ResponseHandler.GetSuccessResponse(response, "Location saved/updated successfully");
    }
}
