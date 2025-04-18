using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Home_Service_Finder.Authentication.Contracts;
using Home_Service_Finder.Data;
using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Roles;
using Home_Service_Finder.Users.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Home_Service_Finder.Authentication
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IUnitOfWork _dbContext;

        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions, IUnitOfWork dbContext)
        {
            _jwtOptions = jwtOptions.Value;
            _dbContext = dbContext;
            

            if (string.IsNullOrEmpty(_jwtOptions.Secret))
            {
                throw new ArgumentException("Jwt Secret Key is missing or empty. Check your configuration");
            }
        }
        public string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret); // strore the values we want to store in secret key.
            var claimList = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                //new Claim(JwtRegisteredClaimNames.,user.IsEmailVerified),
            };

            var role =   _dbContext.Roles.GetByIdAsync(user.RoleId).Result;
            if (role != null)
            {
                claimList.Add(new Claim(ClaimTypes.Role, role.Name));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtOptions.Audience,
                Issuer = _jwtOptions.Issuer,
                Subject = new ClaimsIdentity(claimList),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);



        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            // In a real implementation, you might want to validate the refresh token format
            // or check if it's in a blacklist (for revoked tokens)
            return !string.IsNullOrWhiteSpace(refreshToken);
        }
    }
}
