using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Home_Service_Finder.Users;
using Home_Service_Finder.Users.Users;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Home_Service_Finder.Authentication
{
    [Table("RefreshToken", Schema = "Users")]

    public class RefreshToken
    {
        [Key]
        [Column("Id", TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Column(TypeName = "text")]
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpriesOnUtc { get; set; }
        [ForeignKey("UserId")]
        public  User User { get; set; }
    }
}
