using Home_Service_Finder.Data;
using Home_Service_Finder.Users.Users.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Home_Service_Finder.Users.Users
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        
        internal DbSet<User> _dbSet;

        public UserRepository(AppDbContext db) : base(db)
        {
            _dbSet = db.Set<User>();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted);
            return entity;
        }

        public async Task<User> GetByPhoneNumber(string phoneNumber)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber && !x.IsDeleted);
            return entity;
        }

        //public async Task<User> GetByUserNameAsync(string username)
        //{
        //    var entity = await _dbSet.FirstOrDefaultAsync(x => x.Username == username && !x.IsDeleted);
        //    return entity;
        //}

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return entity;
        }
    }
    
}
