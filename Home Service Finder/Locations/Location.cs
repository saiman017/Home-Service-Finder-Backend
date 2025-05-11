using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Home_Service_Finder.Users.Users;

namespace Home_Service_Finder.Locations
{
    [Table("Location", Schema = "Locations")]
    public class Location
    {
        [Key]
        [Column("UserId", TypeName = "uuid")]
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [Column("Address", TypeName = "varchar")]
        public string Address { get; set; }

        [Column("City", TypeName = "varchar")]
        public string? City { get; set; }

        [Column("PostalCode", TypeName = "varchar")]
        public string? PostalCode { get; set; }

        [Column("Latitude", TypeName = "double precision")]
        public double Latitude { get; set; }

        [Column("Longitude", TypeName = "double precision")]
        public double Longitude { get; set; }
        [Column("CreatedAt", TypeName = "timestamptz")] 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt", TypeName = "timestamptz")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        public User User { get; set; }
    }
}
