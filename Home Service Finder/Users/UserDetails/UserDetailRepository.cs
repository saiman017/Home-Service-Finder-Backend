using Home_Service_Finder.Data;
using Home_Service_Finder.Users.UserDetails.Contracts;

namespace Home_Service_Finder.Users.UserDetails
{
    public class UserDetailRepository : GenericRepository<UserDetail>, IUserDetailRepository
    {
        public UserDetailRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
