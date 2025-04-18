using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Home_Service_Finder.Users.Users;
namespace Home_Service_Finder.Email
{
    [Table("EmailOTP", Schema = "Users")]
    public class EmailOTP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id", TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("UserId", TypeName = "uuid")]
        public Guid UserId { get; set; }

        [Column("Code", TypeName = "VARCHAR(6)")]
        [Required]
        public string Code { get; set; }

        [Column("ExpiryTime", TypeName = "TIMESTAMPTZ")]
        public DateTime ExpiryTime { get; set; }

        [Column("IsUsed", TypeName = "BOOLEAN")]
        public bool IsUsed { get; set; } = false;

        [Column("CreatedAt", TypeName = "TIMESTAMPTZ")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
