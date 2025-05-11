using Home_Service_Finder.Data;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Users.Dtos;
using Home_Service_Finder.Users.ServiceProvider.Contracts;
using Home_Service_Finder.Users.ServiceProvider.Dtos;
using Home_Service_Finder.Users.UserDetails;
using Home_Service_Finder.Users.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Home_Service_Finder.Users.ServiceProvider
{
    public class ServiceProviderService : IServiceProviderService
    {

        private readonly IUnitOfWork _db;
        private readonly AppDbContext _dbContext;

        public ServiceProviderService(IUnitOfWork unitOfWork, AppDbContext dbContext)
        {
            _db = unitOfWork;
            _dbContext = dbContext;
            
        }
        public async Task<APIResponse> AddServiceProviderAsync(ServiceProviderRequestDto serviceProviderRequestDto)
        {
            var checkEmail = await _db.Users.GetByEmailAsync(serviceProviderRequestDto.Email);
            if( checkEmail != null)
            {
                return ResponseHandler.GetBadRequestResponse("Email already exists");
            }

            var checkPhoneNumber =await _db.Users.GetByPhoneNumber(serviceProviderRequestDto.PhoneNumber);
            if (checkPhoneNumber != null)
            {
                return ResponseHandler.GetBadRequestResponse("PhoneNumber already exists");
            }

            //var checkUsername = await _db.Users.GetByEmailAsync(serviceProviderRequestDto.Username);
            //if (checkUsername != null)
            //{
            //    return ResponseHandler.GetBadRequestResponse("Username already exists");
            //}

            if (serviceProviderRequestDto.Password != serviceProviderRequestDto.ConfirmPassword)
            {
                return ResponseHandler.GetBadRequestResponse("Password and Confirm password do not match");
            }

            // add more validation role where only service provider 
            var role = await _db.Roles.GetByIdAsync(serviceProviderRequestDto.RoleId);
            if (role == null)
            {
                return ResponseHandler.GetBadRequestResponse("Role does not exist");
            }

            var serviceCategory = await _db.ServiceCategories.GetByIdAsync(serviceProviderRequestDto.ServiceCategoryId);
            if(serviceCategory == null)
            {
                return ResponseHandler.GetBadRequestResponse("Service Category does not exist");

            }

            User user = new User()
            {
                Email = serviceProviderRequestDto.Email,
                //Username = serviceProviderRequestDto.Username,
                PhoneNumber = serviceProviderRequestDto.PhoneNumber,
                Password = serviceProviderRequestDto.Password,
                RoleId = serviceProviderRequestDto.RoleId, // Directly assign RoleId
                CreatedAt = DateTime.UtcNow,

            };

            user = await _db.Users.AddAsync(user);


            UserDetail userDetail = new UserDetail()
            {
                Id = user.Id,
                FirstName = serviceProviderRequestDto.FirstName,
                //MiddleName = serviceProviderRequestDto.MiddleName,
                LastName = serviceProviderRequestDto.LastName,
                Gender = serviceProviderRequestDto.Gender,
                DateOfBirth = serviceProviderRequestDto.DateOfBirth,
                ProfilePicture = serviceProviderRequestDto.ProfilePicture
            };

            userDetail = await _db.UserDetails.AddAsync(userDetail);

            ServiceProvider serviceProvider = new ServiceProvider()
            {
                Id = user.Id,
                User = user,
                //Licenses = serviceProviderRequestDto.Licenses,
                Experience = serviceProviderRequestDto.Experience,
                PersonalDescription = serviceProviderRequestDto.PersonalDescription,
                ServiceCategoryId = serviceProviderRequestDto.ServiceCategoryId
            };

            serviceProvider = await _db.ServiceProviders.AddAsync(serviceProvider);

            string result = await _db.SaveChangesAsync();

            var serviceCategoryName = serviceCategory.Name;

            ServiceProviderResponseDto respone = new ServiceProviderResponseDto()
            {
                Id = user.Id,
                Email = user.Email,
                //Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                FirstName = userDetail.FirstName,
                //MiddleName = userDetail.MiddleName,
                LastName = userDetail.LastName,
                Gender = userDetail.Gender,
                DateOfBirth = userDetail.DateOfBirth,
                Role = role.Name,
                ProfilePicture = userDetail.ProfilePicture,
                CreatedAt = user.CreatedAt,
                //Licenses = serviceProvider.Licenses,
                ModifiedAt = user.ModifiedAt,
                Experience = serviceProvider.Experience,
                PersonalDescription = serviceProvider.PersonalDescription,
                ServiceCategoryId = serviceProvider.ServiceCategoryId,
                ServiceCategory = serviceCategoryName,
                IsEmailVerified = user.IsEmailVerified,
                IsAdminVerified = serviceProvider.IsAdminVerified,
                IsActive = serviceProvider.IsActive
                //ServiceCategoryId = serviceProvider.ServiceCategoryId,
                //IsDeleted = user.IsDeleted

            };

            return ResponseHandler.GetSuccessResponse(respone, "Service Provider added Successfully");
        }

        public async Task<APIResponse> DeleteServiceProviderAsync(Guid id)
        {
            var serviceProvider = await _db.ServiceProviders.GetByIdAsync(id);
            var user = await _db.Users.GetByIdAsync(id);
            if (serviceProvider == null || user.IsDeleted || user == null)
            {
                return ResponseHandler.GetNotFoundResponse("Service Provider not found");
            }
            user.IsDeleted = true; //soft delete

            user =  _db.Users.UpdateAsync(user);


            string result = await _db.SaveChangesAsync();

            return ResponseHandler.GetSuccessResponse(serviceProvider, "Service Provider deleted successfully");
        }

        public async Task<APIResponse> GetAllServiceProviderAsync()
        {
            var serviceProviders = await _dbContext.Set<ServiceProvider>()
       .Include(sp => sp.User)
       .Where(sp => !sp.User.IsDeleted)
       .ToListAsync();


            var response = new List<ServiceProviderResponseDto>();

            foreach (var sp in serviceProviders)
            {
                var user = await _db.Users.GetByIdAsync(sp.Id);
                var userDetail = await _db.UserDetails.GetByIdAsync(sp.Id);
                var role = await _db.Roles.GetByIdAsync(user.RoleId);
                var serviceCategory = await _db.ServiceCategories.GetByIdAsync(sp.ServiceCategoryId);

                response.Add(new ServiceProviderResponseDto
                {
                    Id = sp.Id,
                    Email = user.Email,
                    //Username = user.Username,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = userDetail?.FirstName,
                    LastName = userDetail?.LastName,
                    Gender = userDetail?.Gender,
                    DateOfBirth = userDetail?.DateOfBirth ?? default,
                    Role = role?.Name,
                    ProfilePicture = userDetail?.ProfilePicture,
                    CreatedAt = user.CreatedAt,
                    IsEmailVerified = user.IsEmailVerified,
                    IsAdminVerified = sp.IsAdminVerified,
                    ModifiedAt = user.ModifiedAt,
                    Experience = sp.Experience,
                    PersonalDescription = sp.PersonalDescription,
                    ServiceCategoryId = sp.ServiceCategoryId,

                    ServiceCategory = serviceCategory?.Name,
                    IsActive = sp.IsActive
                });
            }

            return ResponseHandler.GetSuccessResponse(response);

        }

        public async Task<APIResponse> GetServiceProviderById(Guid id)
        {
            var serviceProvider = await _db.ServiceProviders.GetByIdAsync(id);
            var user = await _db.Users.GetByIdAsync(id);

            if (serviceProvider == null || user.IsDeleted  || user == null)
            {
                return ResponseHandler.GetNotFoundResponse("Service provider not found");
            }

            var userDetail = await _db.UserDetails.GetByIdAsync(id);
            var role = await _db.Roles.GetByIdAsync(user.RoleId);
            var serviceCategory = await _db.ServiceCategories.GetByIdAsync(serviceProvider.ServiceCategoryId);

            var response = new ServiceProviderResponseDto
            {
                Id = serviceProvider.Id,
                Email = user.Email,
                //Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                FirstName = userDetail?.FirstName,
                //MiddleName = userDetail?.MiddleName,
                LastName = userDetail?.LastName,
                Gender = userDetail?.Gender,
                DateOfBirth = userDetail?.DateOfBirth ?? default,
                Role = role?.Name,
                ProfilePicture = userDetail?.ProfilePicture,
                CreatedAt = user.CreatedAt,
                //Licenses = serviceProvider.Licenses,
                IsEmailVerified = user.IsEmailVerified,
                IsAdminVerified = serviceProvider.IsAdminVerified,
                ModifiedAt = user.ModifiedAt,
                Experience = serviceProvider.Experience,
                PersonalDescription = serviceProvider.PersonalDescription,
                ServiceCategoryId = serviceProvider.ServiceCategoryId,
                ServiceCategory = serviceCategory?.Name,
                //IsDocumentVerified = serviceProvider.IsDocumentVerified,
                IsActive = serviceProvider.IsActive
            };

            return ResponseHandler.GetSuccessResponse(response);
        }

        public async Task<APIResponse> UpdateServiceProvider(Guid id, ServiceProviderUpdateRequestDto serviceProviderUpdateRequestDto)
        {
            var serviceProvider = await _db.ServiceProviders.GetByIdAsync(id);
            var user = await _db.Users.GetByIdAsync(id);

            if (serviceProvider == null || user.IsDeleted || user == null)
            {
                return ResponseHandler.GetNotFoundResponse("Service provider not found");
            }
            var userDetail = await _db.UserDetails.GetByIdAsync(id);

            // Validate email uniqueness if changed
            if (user.Email != serviceProviderUpdateRequestDto.Email)
            {
                var emailExists = await _db.Users.GetByEmailAsync(serviceProviderUpdateRequestDto.Email);
                if (emailExists != null)
                {
                    return ResponseHandler.GetBadRequestResponse("Email already in use");
                }
            }

            // Validate phone uniqueness if changed
            if (user.PhoneNumber != serviceProviderUpdateRequestDto.PhoneNumber)
            {
                var phoneExists = await _db.Users.GetByPhoneNumber(serviceProviderUpdateRequestDto.PhoneNumber);
                if (phoneExists != null)
                {
                    return ResponseHandler.GetBadRequestResponse("Phone number already in use");
                }
            }

           
            user.Email = serviceProviderUpdateRequestDto.Email;
            //user.Username = serviceProviderUpdateRequestDto.Username;
            user.PhoneNumber = serviceProviderUpdateRequestDto.PhoneNumber;
            user.ModifiedAt = DateTime.UtcNow;
            user = _db.Users.UpdateAsync(user);

            // Update user details
            if (userDetail != null)
            {
                userDetail.FirstName = serviceProviderUpdateRequestDto.FirstName;
                //userDetail.MiddleName = serviceProviderUpdateRequestDto.MiddleName;
                userDetail.LastName = serviceProviderUpdateRequestDto.LastName;
                userDetail.Gender = serviceProviderUpdateRequestDto.Gender;
                userDetail.DateOfBirth = serviceProviderUpdateRequestDto.DateOfBirth;
                userDetail.ProfilePicture = serviceProviderUpdateRequestDto.ProfilePicture;
                userDetail = _db.UserDetails.UpdateAsync(userDetail);
            }

            // Update service provider
            //serviceProvider.Licenses = serviceProviderUpdateRequestDto.Licenses;
            serviceProvider.Experience = serviceProviderUpdateRequestDto.Experience;
            serviceProvider.PersonalDescription = serviceProviderUpdateRequestDto.PersonalDescription;
            //serviceProvider.ServiceCategoryId = serviceProviderUpdateRequestDto.ServiceCategoryId;
            serviceProvider =  _db.ServiceProviders.UpdateAsync(serviceProvider);

            // Save changes
            string result = await _db.SaveChangesAsync();

            // Prepare response
            var role = await _db.Roles.GetByIdAsync(user.RoleId);
            var serviceCategory = await _db.ServiceCategories.GetByIdAsync(serviceProvider.ServiceCategoryId);

            var response = new ServiceProviderResponseDto
            {
                Id = serviceProvider.Id,
                Email = user.Email,
                //Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                FirstName = userDetail.FirstName,
                //MiddleName = userDetail?.MiddleName,
                LastName = userDetail.LastName,
                Gender = userDetail.Gender,
                DateOfBirth = userDetail.DateOfBirth,
                Role = role?.Name,
                ProfilePicture = userDetail?.ProfilePicture,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt,
                Experience = serviceProvider.Experience,
                PersonalDescription = serviceProvider.PersonalDescription,
                ServiceCategoryId = serviceProvider.ServiceCategoryId,
                ServiceCategory = serviceCategory.Name,
                //IsDocumentVerified = serviceProvider.IsDocumentVerified,
                IsEmailVerified = user.IsEmailVerified,
                IsAdminVerified = serviceProvider.IsAdminVerified,
                IsActive = serviceProvider.IsActive
            };

            return ResponseHandler.GetSuccessResponse(response, "Service provider updated successfully");
        }

        public async Task<APIResponse> GetServiceProviderStatisticsAsync(Guid providerId)
        {
            var serviceProvider = await _db.ServiceProviders.GetByIdAsync(providerId);
            if (serviceProvider == null)
                return ResponseHandler.GetNotFoundResponse("Service provider not found.");

            // Convert to Nepal time (UTC+5:45)
            var nepalTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            var currentUtcTime = DateTime.UtcNow;
            var nepalTimeNow = TimeZoneInfo.ConvertTimeFromUtc(currentUtcTime, nepalTimeZone);

            var todayNepal = nepalTimeNow.Date;
            var startOfWeekNepal = todayNepal.AddDays(-(int)todayNepal.DayOfWeek); // Sunday as start of week

            var offers = (await _db.ServiceOffers.GetAllAsync())
                .Where(so => so.ServiceProviderId == providerId && so.Status == "Completed")
                .ToList();

            var totalEarnings = offers.Sum(o => o.OfferedPrice);
            var totalCompletedOffers = offers.Count;

            var todayEarnings = offers
                .Where(o =>
                {
                    var offerNepalTime = TimeZoneInfo.ConvertTimeFromUtc(o.SentAt, nepalTimeZone);
                    return offerNepalTime.Date == todayNepal;
                })
                .Sum(o => o.OfferedPrice);

            var weekEarnings = offers
                .Where(o =>
                {
                    var offerNepalTime = TimeZoneInfo.ConvertTimeFromUtc(o.SentAt, nepalTimeZone);
                    return offerNepalTime.Date >= startOfWeekNepal;
                })
                .Sum(o => o.OfferedPrice);

            var todayCompletedOffers = offers
                .Where(o =>
                {
                    var offerNepalTime = TimeZoneInfo.ConvertTimeFromUtc(o.SentAt, nepalTimeZone);
                    return offerNepalTime.Date == todayNepal;
                })
                .Count();

            var statistics = new
            {
                TotalEarnings = totalEarnings,
                TotalCompletedOffers = totalCompletedOffers,
                TotalRevenueToday = todayEarnings,
                TotalRevenueThisWeek = weekEarnings,
                TotalOffersCompletedToday = todayCompletedOffers
            };

            return ResponseHandler.GetSuccessResponse(statistics, "Service provider statistics retrieved successfully.");
        }

        public async Task<APIResponse> GetRevenueTimeSeriesAsync(Guid providerId, string groupBy = "month")
        {
            var offers = await _db.ServiceOffers.GetAllAsync();
            var paid = offers
                .Where(o => o.ServiceProviderId == providerId && o.PaymentStatus && o.Status == "Completed")
                .ToList();

            IEnumerable<ServiceProviderRevenueDto> series = groupBy.ToLower() switch
            {
                "day" => paid
                    .GroupBy(o => o.SentAt.Date)
                    .Select(g => new ServiceProviderRevenueDto(
                        g.Key.ToString("yyyy-MM-dd"),
                        g.Sum(o => o.OfferedPrice)
                    )),
                "week" => paid
                    .GroupBy(o => new {
                        Year = o.SentAt.Year,
                        Week = (o.SentAt.DayOfYear - 1) / 7 + 1
                    })
                    .Select(g => new ServiceProviderRevenueDto(
                        $"{g.Key.Year:0000}-W{g.Key.Week:00}",
                        g.Sum(o => o.OfferedPrice)
                    )),
                _ => // "month"
                    paid
                    .GroupBy(o => new {
                        Year = o.SentAt.Year,
                        Month = o.SentAt.Month
                    })
                    .Select(g => new ServiceProviderRevenueDto(
                        new DateTime(g.Key.Year, g.Key.Month, 1).ToString("yyyy-MM"),
                        g.Sum(o => o.OfferedPrice)
                    ))
            };

            var ordered = series.OrderBy(s => s.Period).ToList();
            return ResponseHandler.GetSuccessResponse(ordered);
        }


    }
    public record ServiceProviderRevenueDto(string Period, decimal Amount);
}
