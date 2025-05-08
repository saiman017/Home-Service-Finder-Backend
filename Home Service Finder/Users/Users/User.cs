using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Home_Service_Finder.Locations;
using Home_Service_Finder.Roles;
using Home_Service_Finder.Users.UserDetails;

namespace Home_Service_Finder.Users.Users
{
    [Table("User", Schema = "Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id", TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("Email", TypeName = "VARCHAR(100)")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //[Column("Username", TypeName = "VARCHAR(50)")]
        //[Required]
        //public string Username { get; set; }

        [Column("PhoneNumber", TypeName = "VARCHAR(50)")]
        [Required]
        public string PhoneNumber { get; set; }
       
        [Column("Password", TypeName = "VARCHAR(20)")]
        [Required]
        public string Password { get; set; }

        [Column("CreatedAt", TypeName = "TIMESTAMPTZ")] 
        public DateTime CreatedAt { get; set; } 

        [Column("ModifiedAt", TypeName = "TIMESTAMPTZ")]
        public DateTime ModifiedAt { get; set; }

        [Column("IsDeleted", TypeName = "BOOLEAN")]
        public bool IsDeleted { get; set; } = false;

        [Column("IsEmailVerified", TypeName = "BOOLEAN")]
        public bool IsEmailVerified { get; set; } = false;

        [ForeignKey("Role")]  
        public Guid RoleId { get; set; }

        public virtual Role Role { get; set; }

        public Location Location { get; set; }

        public virtual UserDetail UserDetail { get; set; }

    }
}