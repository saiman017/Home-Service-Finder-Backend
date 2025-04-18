using Home_Service_Finder.Data;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Users.Dtos;
using Home_Service_Finder.Users.ServiceProvider.Contracts;
using Home_Service_Finder.Users.ServiceProvider.Dtos;
using Home_Service_Finder.Users.UserDetails;
using Home_Service_Finder.Users.Users;
using Microsoft.EntityFrameworkCore;

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

            //// Validate username uniqueness if changed
            //if (user.Username != serviceProviderUpdateRequestDto.Username)
            //{
            //    var usernameExists = await _db.Users.GetByUserNameAsync(serviceProviderUpdateRequestDto.Username);
            //    if (usernameExists != null)
            //    {
            //        return ResponseHandler.GetBadRequestResponse("Username already in use");
            //    }
            //}

            // Validate service category exists if changed
            //if (serviceProvider.ServiceCategoryId != serviceProviderUpdateRequestDto.ServiceCategoryId)
            //{
            //    var categoryExists = await _db.ServiceCategories.GetByIdAsync(requestDto.ServiceCategoryId);
            //    if (categoryExists == null)
            //    {
            //        return ResponseHandler.GetBadRequestResponse("Service category does not exist");
            //    }
            //}

            // Update user
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
                ServiceCategory = serviceCategory.Name,
                //IsDocumentVerified = serviceProvider.IsDocumentVerified,
                IsEmailVerified = user.IsEmailVerified,
                IsAdminVerified = serviceProvider.IsAdminVerified,
                IsActive = serviceProvider.IsActive
            };

            return ResponseHandler.GetSuccessResponse(response, "Service provider updated successfully");
        }
    }
}
