using Azure.Core;
using Home_Service_Finder.Authentication.Contracts;
using Home_Service_Finder.Authentication.Dtos;
using Home_Service_Finder.Authentication.Handler;
using Home_Service_Finder.Data;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Email.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Home_Service_Finder.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _db;
        private readonly AppDbContext _dbContext;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IEmailOTPService _emailOTPService;
        bool isPasswordValid;

        public AuthService(IUnitOfWork db,AppDbContext dbContext, IJwtTokenGenerator jwtTokenGenerator, IEmailOTPService emailOTPService)
        {
            _db = db;
            _dbContext = dbContext;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailOTPService = emailOTPService;
        }

        public async Task<APIResponse> Login(LoginRequestDto requestDto)
        {
            var user = await _db.Users.GetByEmailAsync(requestDto.Email);
            if (user == null)
            {
                return ResponseHandler.GetBadRequestResponse("User not found");
            }

            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(requestDto.Password, user.Password);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Plain text fallback
                isPasswordValid = (requestDto.Password == user.Password);

                // If valid, upgrade to hashed password
                if (isPasswordValid)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    _db.Users.UpdateAsync(user);
                    await _db.SaveChangesAsync();
                }
            }

            if (!isPasswordValid)
            {
                return ResponseHandler.GetBadRequestResponse("Invalid password");
            }


            // Check if email is verified
            if (!user.IsEmailVerified)
            {
              return  await _emailOTPService.GenerateOTP(user.Id);         
            }
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);

            var existingToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt =>rt.UserId == user.Id && rt.ExpriesOnUtc > DateTime.UtcNow);


            string refreshToken;
            if (existingToken != null)
            {
   
                refreshToken = existingToken.Token;
                existingToken.ExpriesOnUtc = DateTime.UtcNow.AddDays(6);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
                var userRefreshToken = new RefreshToken()
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    ExpriesOnUtc = DateTime.UtcNow.AddDays(6)
                };
                await _dbContext.RefreshTokens.AddAsync(userRefreshToken);
                await _dbContext.SaveChangesAsync();
            }

            var role = _db.Roles.GetByIdAsync(user.RoleId).Result;


            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                Role = role.Name,
                Email = user.Email,
                UserId = user.Id,   
                IsEmailVerified = user.IsEmailVerified,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return ResponseHandler.GetSuccessResponse(EncodingHandler.EncodeToBase64(loginResponseDto), "Login successfully");
        
        }

        public async Task<APIResponse> LoginWithRefreshToken(string refreshToken)
        {
            // Get the refresh token from database
            RefreshToken? storedRefreshToken = await _dbContext.RefreshTokens
               .Include(r => r.User)
               .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return ResponseHandler.GetUnauthorizedResponse("Invalid refresh token");
            }

            if (storedRefreshToken.ExpriesOnUtc < DateTime.UtcNow)
            {
                return ResponseHandler.GetUnauthorizedResponse("Refresh token expired");
            }

            // Get the user associated with the refresh token
            var user = storedRefreshToken.User;
            if (user == null)
            {
                return ResponseHandler.GetUnauthorizedResponse("User not found");
            }

            // Generate new tokens
            var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            // Revoke the old refresh token
            _dbContext.RefreshTokens.Remove(storedRefreshToken);

            // Add the new refresh token
            var userRefreshToken = new RefreshToken()
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpriesOnUtc = DateTime.UtcNow.AddDays(6)
            };

            await _dbContext.RefreshTokens.AddAsync(userRefreshToken);
            await _dbContext.SaveChangesAsync();


            RefreshTokenResponseDto refreshTokenResponseDto = new RefreshTokenResponseDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
            var responseData = new
            {
                data = refreshTokenResponseDto,
                message = "Token refreshed successfully"
            };

            return ResponseHandler.GetSuccessResponse(EncodingHandler.EncodeToBase64(responseData), "Token refreshed successfully");
        }

    }
}
