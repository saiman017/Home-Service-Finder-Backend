using Azure;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Email.Contracts;
using Home_Service_Finder.Users.Contracts;
using Home_Service_Finder.Users.DTO;
using Home_Service_Finder.Users.Dtos;
using Home_Service_Finder.Users.UserDetails;
using Home_Service_Finder.Users.Users;

namespace Home_Service_Finder.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _db;
        private readonly IEmailOTPService _emailOTPService;

        public UserService(IUnitOfWork unitOfWork, IEmailOTPService emailOTPService)
        {
            _db = unitOfWork;
            _emailOTPService = emailOTPService;
   
        }
        public async Task<APIResponse> AddUser(UserRequestDto userRequestDto)
        {
           
            var existingUserByEmail = await _db.Users.GetByEmailAsync(userRequestDto.Email);
            if (existingUserByEmail != null)
            {
                return ResponseHandler.GetBadRequestResponse("Email already exists");
            }

            var existingUserByPhone = await _db.Users.GetByPhoneNumber(userRequestDto.PhoneNumber);
            if (existingUserByPhone != null)
            {
                return ResponseHandler.GetBadRequestResponse("Phone number already exists");
            }

            //// Check if username already exists
            //var existingUserByUsername = await _db.Users.GetByUserNameAsync(userRequestDto.Username);
            //if (existingUserByUsername != null)
            //{
            //    return ResponseHandler.GetBadRequestResponse("Username already exists");
            //}

            if (userRequestDto.Password != userRequestDto.ConfirmPassword)
            {
                return ResponseHandler.GetBadRequestResponse("Passwords do not match");
            }

            var role = await _db.Roles.GetByIdAsync(userRequestDto.RoleId);
            if (role == null)
            {
                return ResponseHandler.GetBadRequestResponse("Specified role does not exist");
            }

            User user = new User()
            {
                Email = userRequestDto.Email,
                PhoneNumber = userRequestDto.PhoneNumber,
                Password = BCrypt.Net.BCrypt.HashPassword(userRequestDto.Password),
                RoleId = userRequestDto.RoleId,
                CreatedAt = DateTime.UtcNow,
                IsEmailVerified = false

            };

            user = await _db.Users.AddAsync(user);
            UserDetail userDetail = new UserDetail()
            {
                Id = user.Id,
                FirstName = userRequestDto.FirstName,
                //MiddleName = userRequestDto.MiddleName,
                LastName = userRequestDto.LastName,
                Gender = userRequestDto.Gender,
                DateOfBirth = userRequestDto.DateOfBirth,
                ProfilePicture = userRequestDto.ProfilePicture
            };

            userDetail = await _db.UserDetails.AddAsync(userDetail);

            string result = await _db.SaveChangesAsync();

            // Gererate Otp
            await _emailOTPService.GenerateOTP(user.Id);

            UserResponseDto response = new UserResponseDto()
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
                Role = role.Name, // Use role name from the role we fetched earlier
                ProfilePicture = userDetail.ProfilePicture,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt, //no modifiy dat awhen the user is added only in update
                //IsDeleted = user.IsDeleted,
                IsEmailVerified = user.IsEmailVerified,
            };

            return ResponseHandler.GetSuccessResponse(response, "User added successfully");
        }

        public async Task<APIResponse> DeleteUser(Guid id)
        {
            var user = await _db.Users.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                return ResponseHandler.GetNotFoundResponse("User of not found");
            }
            user.IsDeleted = true; 

            user = _db.Users.UpdateAsync(user);


            string result = await _db.SaveChangesAsync();

            return ResponseHandler.GetSuccessResponse(user, "User deleted successfully");

        }

        public async Task<APIResponse> GetAllUsers()
        {
            var users = (await _db.Users.GetAllAsync())
                .Where(u => !u.IsDeleted )
                .ToList();

            var response = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var userDetail = await _db.UserDetails.GetByIdAsync(user.Id);

                var role = await _db.Roles.GetByIdAsync(user.RoleId);
                var roleName = role?.Name ?? "Unknown";

                var userResponse = new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    //Username = user.Username,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    ModifiedAt = user.ModifiedAt,
                    //IsDeleted = user.IsDeleted,
                    Role = roleName,
                    IsEmailVerified = user.IsEmailVerified
                };

                if (userDetail != null)
                {
                    userResponse.FirstName = userDetail.FirstName;
                    //userResponse.MiddleName = userDetail.MiddleName;
                    userResponse.LastName = userDetail.LastName;
                    userResponse.Gender = userDetail.Gender;
                    userResponse.DateOfBirth = userDetail.DateOfBirth;
                    userResponse.ProfilePicture = userDetail.ProfilePicture;
                }

                response.Add(userResponse);
            }

            return ResponseHandler.GetSuccessResponse(response);
        }


        public async Task<APIResponse> GetUserById(Guid id)
        {
           
            var user = await _db.Users.GetByIdAsync(id);

            if (user == null || user.IsDeleted)
            {
                return ResponseHandler.GetNotFoundResponse("User not found");
            }

            var userDetail = await _db.UserDetails.GetByIdAsync(user.Id);

            var role = await _db.Roles.GetByIdAsync(user.RoleId);
            var roleName = role?.Name ?? "Unknown";

            var response = new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                //Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt,
                //IsDeleted = user.IsDeleted,
                Role = roleName,
                IsEmailVerified = user.IsEmailVerified
            };

            if (userDetail != null)
            {
                response.FirstName = userDetail.FirstName;
                //response.MiddleName = userDetail.MiddleName;
                response.LastName = userDetail.LastName;
                response.Gender = userDetail.Gender;
                response.DateOfBirth = userDetail.DateOfBirth;
                response.ProfilePicture = userDetail.ProfilePicture;
            }

            return ResponseHandler.GetSuccessResponse(response);
        }

        public async Task<APIResponse> UpdateUser(Guid id, UserUpdateRequestDto userUpdateRequestDto)
        {

            var user = await _db.Users.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                return ResponseHandler.GetNotFoundResponse("User not found");
            }

            if (user.Email != userUpdateRequestDto.Email)
            {
                var emailExists = await _db.Users.GetByEmailAsync(userUpdateRequestDto.Email);
                if (emailExists != null)
                {
                    return ResponseHandler.GetBadRequestResponse("Email already in use");
                }
            }

            //// Validate username uniqueness if changed
            //if (user.Username != userUpdateRequestDto.Username)
            //{
            //    var usernameExists = await _db.Users.GetByUserNameAsync(userUpdateRequestDto.Username);
            //    if (usernameExists != null)
            //    {
            //        return ResponseHandler.GetBadRequestResponse("Username already in use");
            //    }
            //}

            // Validate phone number uniqueness if changed
            if (user.PhoneNumber != userUpdateRequestDto.PhoneNumber)
            {
                var phoneExists = await _db.Users.GetByPhoneNumber(userUpdateRequestDto.PhoneNumber);
                if (phoneExists != null)
                {
                    return ResponseHandler.GetBadRequestResponse("Phone number already in use");
                }
            }

            user.Email = userUpdateRequestDto.Email;
            //user.Username = userUpdateRequestDto.Username;
            user.PhoneNumber = userUpdateRequestDto.PhoneNumber;
            user.ModifiedAt = DateTime.UtcNow;

            user =  _db.Users.UpdateAsync(user);

            var userDetail = await _db.UserDetails.GetByIdAsync(id) ?? new UserDetail { Id = id };

            userDetail.FirstName = userUpdateRequestDto.FirstName;
            //userDetail.MiddleName = userUpdateRequestDto.MiddleName;
            userDetail.LastName = userUpdateRequestDto.LastName;
            userDetail.Gender = userUpdateRequestDto.Gender;
            userDetail.DateOfBirth = userUpdateRequestDto.DateOfBirth;
            userDetail.ProfilePicture = userUpdateRequestDto.ProfilePicture;

            userDetail =  _db.UserDetails.UpdateAsync(userDetail);

            string result = await _db.SaveChangesAsync();

            var role = await _db.Roles.GetByIdAsync(user.RoleId);
            var roleName = role?.Name ?? string.Empty;

            var response = new UserResponseDto
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
                ProfilePicture = userDetail.ProfilePicture,
                Role = roleName,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt,
                IsEmailVerified = user.IsEmailVerified
                //IsDeleted = user.IsDeleted
            };

            return result == "Successfully saved!"
                ? ResponseHandler.GetSuccessResponse(response, "User updated successfully")
                : ResponseHandler.GetBadRequestResponse("Failed to update user");
        }

        public async Task<APIResponse> UploadProfilePicture(Guid userId, IFormFile file)
        {
            var userDetail = await _db.UserDetails.GetByIdAsync(userId);
            if (userDetail == null)
                return ResponseHandler.GetNotFoundResponse("User not found");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if (!string.IsNullOrEmpty(userDetail.ProfilePicture))
            {
                var oldImagePath = Path.Combine(uploadsFolder, Path.GetFileName(userDetail.ProfilePicture));
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
            }

            userDetail.ProfilePicture = $"/uploads/{fileName}";
            _db.UserDetails.UpdateAsync(userDetail);
            await _db.SaveChangesAsync();

            return ResponseHandler.GetSuccessResponse(userDetail.ProfilePicture, "Profile picture uploaded successfully");
        }

        public async Task<APIResponse> DeleteProfilePicture(Guid userId)
        {
            var userDetail = await _db.UserDetails.GetByIdAsync(userId);
            if (userDetail == null || string.IsNullOrEmpty(userDetail.ProfilePicture))
                return ResponseHandler.GetNotFoundResponse("Profile picture not found");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            var imagePath = Path.Combine(uploadsFolder, Path.GetFileName(userDetail.ProfilePicture));

            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            userDetail.ProfilePicture = null;
            _db.UserDetails.UpdateAsync(userDetail);
            await _db.SaveChangesAsync();

            return ResponseHandler.GetSuccessResponse(null, "Profile picture deleted successfully");
        }

    }
}
