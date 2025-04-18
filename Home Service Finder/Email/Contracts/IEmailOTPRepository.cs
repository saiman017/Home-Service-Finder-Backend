using Home_Service_Finder.Data.Contracts;
using static System.Net.WebRequestMethods;

namespace Home_Service_Finder.Email.Contracts
{
    public interface IEmailOTPRepository : IGenericRepository<EmailOTP>
    {

        Task<EmailOTP?> GetLatestOTPForUserAsync(Guid userId);
        Task<EmailOTP?> GetByCodeAndUserIdAsync(string code, Guid userId);
    }
}
