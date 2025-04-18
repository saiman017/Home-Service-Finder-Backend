using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Home_Service_Finder.Roles
{
    [Table("Role", Schema = "Users")]
    public class Role
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id", TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("Name", TypeName = "VARCHAR(50)")]
        public string Name { get; set; }

        [Column("IsDeleted", TypeName = "BOOLEAN")]

        public bool IsDeleted { get; set; } = false;
    }
}
