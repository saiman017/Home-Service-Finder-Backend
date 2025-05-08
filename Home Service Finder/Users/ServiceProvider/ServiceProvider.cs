using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Home_Service_Finder.ServiceCategories;
using Home_Service_Finder.Users.Users;

namespace Home_Service_Finder.Users.ServiceProvider
{
    [Table("ServiceProvider", Schema = "Users")]

    public class ServiceProvider 
    {
        [Key]
        [Column("Id", TypeName = "uuid")]
        [ForeignKey("User")]
        public Guid Id { get; set; }
        public virtual User User { get; set; } 

        [Column("Experience", TypeName ="INT")]
        public int Experience { get; set; }

        [Column("IsActive", TypeName = "BOOLEAN")]
        public bool IsActive { get; set; } = false; // after login make it true

        [Column("IsAdminVerified", TypeName = "BOOLEAN")]
        public bool IsAdminVerified { get; set; } = false;

        [Column("PersonalDescription", TypeName = "VARCHAR(500)")]
        public string? PersonalDescription { get; set; }

        [ForeignKey("ServiceCategory")]
        public Guid ServiceCategoryId { get; set; }
        public virtual ServiceCategory ServiceCategory { get; set; }



    }
}
