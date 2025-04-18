//using System.Threading.Tasks;
//using Home_Service_Finder.Data;
//using Home_Service_Finder.Users.UserRoles.Contracts;
//using Microsoft.EntityFrameworkCore;


//namespace Home_Service_Finder.Users.UserRoles
//{
//    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
//    {
//        private readonly AppDbContext _dbContext;

//        public UserRoleRepository(AppDbContext dbContext) : base(dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        //public  void DeleteUserRoleByUserId(Guid userId)
//        //{
//        //    var userRole = _dbContext.UserRoles.Where(x => x.UserId == userId).ToList();

//        //    _dbContext.UserRoles.RemoveRange(userRole);
//        //}

//        public void DeleteUserRoleByUserId(Guid userId) // for one to one relation
//        {
//            var userRole = _dbContext.UserRoles.FirstOrDefault(x => x.UserId == userId);

//            _dbContext.UserRoles.Remove(userRole);
//        }

//        public async Task<UserRole> GetUserRoleByUserId(Guid userId)
//        {
//            var result =  await  _dbContext.UserRoles.Include(x => x.Role).FirstOrDefaultAsync(r => r.UserId == userId); // inclucde before exceution
//            if(result == null)
//            {
//                throw new NullReferenceException("Role not found");
//            }

//            return  result;
//        }


//    }
//}
