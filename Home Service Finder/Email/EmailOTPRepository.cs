using Home_Service_Finder.Data;
using Home_Service_Finder.Email.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Home_Service_Finder.Email
{
    public class EmailOTPRepository : GenericRepository<EmailOTP>, IEmailOTPRepository
    {
        private readonly AppDbContext _dbContext;

        public EmailOTPRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<EmailOTP?> GetLatestOTPForUserAsync(Guid userId)
        {
            return await _dbContext.Emails
                .Where(o => o.UserId == userId && !o.IsUsed && o.ExpiryTime > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<EmailOTP?> GetByCodeAndUserIdAsync(string code, Guid userId)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("Code cannot be null or empty", nameof(code));
            }

            return await _dbContext.Emails
                .FirstOrDefaultAsync(o =>
                    o.Code == code &&
                    o.UserId == userId &&
                    !o.IsUsed &&
                    o.ExpiryTime > DateTime.UtcNow);
        }
    }
}