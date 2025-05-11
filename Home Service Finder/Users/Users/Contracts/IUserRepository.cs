using Home_Service_Finder.Data.Contracts;
using Home_Service_Finder.Users.Users;

namespace Home_Service_Finder.Users.Users.Contracts
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByEmailAsync(string email);

        Task<User> GetByPhoneNumber(string phoneNumber);

        Task<User> GetUserByIdAsync(Guid id);
    }
}
