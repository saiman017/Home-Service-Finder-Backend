//using System.ComponentModel.DataAnnotations.Schema;
//using Home_Service_Finder.Roles;
//using Home_Service_Finder.Users.Users;

//namespace Home_Service_Finder.Users.UserRoles
//{
//    [Table("UserRole", Schema="Users")]
//    public class UserRole
//    {
//        [ForeignKey("Role")]
//        public Guid RoleId { get; set; }
//        public virtual Role Role { get; set; }

//        [ForeignKey("User")]
//        public Guid UserId { get; set; }
//        public virtual User User { get; set; }
//    }
//}
