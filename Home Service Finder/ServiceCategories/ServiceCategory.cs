using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using Home_Service_Finder.ServicesList;

namespace Home_Service_Finder.ServiceCategories
{
    [Table("ServieCategory", Schema = "Services")]
    public class ServiceCategory
    {
        [Key]
        [Column("Id", TypeName = "uuid")]
        public Guid Id { get; set; }

        [Column("Name", TypeName = "VARCHAR(50)")]
        public string Name { get; set; }
        [Column("Description", TypeName = "VARCHAR(500)")]
        public string? Description { get; set; }

        [Column("CategoryImage", TypeName = "VARCHAR(500)")]
        public string CategoryImage { get; set; }

        [Column("CreatedAt", TypeName = "TIMESTAMPTZ")]
        public DateTime CreatedAt { get; set; }

        [Column("ModifiedAt", TypeName = "TIMESTAMPTZ")]
        public DateTime ModifiedAt { get; set; }

        [Column("IsDeleted", TypeName = "BOOLEAN")]
        public bool IsDeleted { get; set; } = false;

        public ICollection<ServiceList> Services { get; set; }


    }
}
