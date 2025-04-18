using System.ComponentModel.DataAnnotations.Schema;
using Home_Service_Finder.ServiceCategories;

namespace Home_Service_Finder.ServicesList
{
    [Table("ServiceList", Schema = "Services")]
    public class ServiceList
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id", TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("Name", TypeName = "VARCHAR(250)")]
        public string Name { get; set; }

        [Column("CreatedAt", TypeName = "TIMESTAMPTZ")]
        public DateTime CreatedAt { get; set; }

        [Column("ModifiedAt", TypeName = "TIMESTAMPTZ")]
        public DateTime ModifiedAt { get; set; }

        [ForeignKey("ServiceCategory")]
        public Guid ServiceCategoryId { get; set; }
        public ServiceCategory ServiceCategory { get; set; }

    }
}
