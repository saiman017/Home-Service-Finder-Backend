using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Home_Service_Finder.Users.Users;

namespace Home_Service_Finder.Users.UserDetails
{
    [Table("UserDetail", Schema = "Users")]
    public class UserDetail
    {
        [Key]
        [Column("Id", TypeName = "uuid")]
        [ForeignKey("User")]
        public Guid Id { get; set; } 
        public User User { get; set; }

        [Column("FirstName", TypeName = "VARCHAR(50)")]
        [Required]
        public string FirstName { get; set; }

        [Column("LastName", TypeName = "VARCHAR(50)")]
        [Required]
        public string LastName { get; set; }

        [Column("Gender", TypeName = "VARCHAR(50)")]
        public string Gender { get; set; }

        [Column("DateOfBirth", TypeName = "DATE")]
        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Column("ProfilePicture", TypeName = "VARCHAR(500)")]
        public string? ProfilePicture { get; set; }

    }
}