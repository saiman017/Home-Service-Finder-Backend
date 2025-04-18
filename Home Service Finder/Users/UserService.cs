using Azure;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Email.Contracts;
using Home_Service_Finder.Users.Contracts;
using Home_Service_Finder.Users.DTO;
using Home_Service_Finder.Users.Dtos;
using Home_Service_Finder.Users.UserDetails;
//using Home_Service_Finder.Users.UserRoles;
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
            // Check if email already exists
            var existingUserByEmail = await _db.Users.GetByEmailAsync(userRequestDto.Email);
            if (existingUserByEmail != null)
            {
                return ResponseHandler.GetBadRequestResponse("Email already exists");
            }

            // Check if phone number already exists
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

            // Verify password match
            if (userRequestDto.Password != userRequestDto.ConfirmPassword)
            {
                return ResponseHandler.GetBadRequestResponse("Passwords do not match");
            }

            // Verify role exists
            var role = await _db.Roles.GetByIdAsync(userRequestDto.RoleId);
            if (role == null)
            {
                return ResponseHandler.GetBadRequestResponse("Specified role does not exist");
            }

            // Create new user
            User user = new User()
            {
                Email = userRequestDto.Email,
                //Username = userRequestDto.Username,
                PhoneNumber = userRequestDto.PhoneNumber,
                Password = userRequestDto.Password,
                RoleId = userRequestDto.RoleId, // Directly assign RoleId
                CreatedAt = DateTime.UtcNow,
                IsEmailVerified = false

            };

            user = await _db.Users.AddAsync(user);

            // Create user details
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

            // Save changes
            string result = await _db.SaveChangesAsync();

            // Gererate Otp
            await _emailOTPService.GenerateOTP(user.Id);

            // Prepare response
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
            user.IsDeleted = true; //soft delete

            user = _db.Users.UpdateAsync(user);


            string result = await _db.SaveChangesAsync();

            return ResponseHandler.GetSuccessResponse(user, "User deleted successfully");

        }

        public async Task<APIResponse> GetAllUsers()
        {
            //var roleGuid = new Guid("d4d45e43-7201-4a64-adce-83f24226413a");
            // Get all non-deleted users and include their roles
            var users = (await _db.Users.GetAllAsync())
                .Where(u => !u.IsDeleted )
                .ToList();

            var response = new List<UserResponseDto>();

            foreach (var user in users)
            {
                // Get user details
                var userDetail = await _db.UserDetails.GetByIdAsync(user.Id);

                // Get role name
                var role = await _db.Roles.GetByIdAsync(user.RoleId);
                var roleName = role?.Name ?? "Unknown";

                // Create response DTO
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

                // Add user details if available
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
            // Get user by ID including their role
            var user = await _db.Users.GetByIdAsync(id);

            // Return not found if user doesn't exist or is deleted
            if (user == null || user.IsDeleted)
            {
                return ResponseHandler.GetNotFoundResponse("User not found");
            }

            // Get user details
            var userDetail = await _db.UserDetails.GetByIdAsync(user.Id);

            // Get user's role
            var role = await _db.Roles.GetByIdAsync(user.RoleId);
            var roleName = role?.Name ?? "Unknown";

            // Create response DTO
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

            // Add user details if available
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
            // Get existing user
            var user = await _db.Users.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                return ResponseHandler.GetNotFoundResponse("User not found");
            }

            // Validate email uniqueness if changed
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

            // Update user fields (excluding RoleId - role cannot be updated here)
            user.Email = userUpdateRequestDto.Email;
            //user.Username = userUpdateRequestDto.Username;
            user.PhoneNumber = userUpdateRequestDto.PhoneNumber;
            user.ModifiedAt = DateTime.UtcNow;

            // Update user in database
            user =  _db.Users.UpdateAsync(user);

            // Get or create user details
            var userDetail = await _db.UserDetails.GetByIdAsync(id) ?? new UserDetail { Id = id };

            // Update user details
            userDetail.FirstName = userUpdateRequestDto.FirstName;
            //userDetail.MiddleName = userUpdateRequestDto.MiddleName;
            userDetail.LastName = userUpdateRequestDto.LastName;
            userDetail.Gender = userUpdateRequestDto.Gender;
            userDetail.DateOfBirth = userUpdateRequestDto.DateOfBirth;
            userDetail.ProfilePicture = userUpdateRequestDto.ProfilePicture;

            // Save user details
            userDetail =  _db.UserDetails.UpdateAsync(userDetail);

            // Save changes
            string result = await _db.SaveChangesAsync();

            // Get role for response
            var role = await _db.Roles.GetByIdAsync(user.RoleId);
            var roleName = role?.Name ?? string.Empty;

            // Prepare response
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
    }
}
