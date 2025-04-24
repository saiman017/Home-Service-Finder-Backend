using Microsoft.EntityFrameworkCore;
using Home_Service_Finder.Authentication;
using Home_Service_Finder.Users.UserDetails;
using Home_Service_Finder.Users.Users;
using Home_Service_Finder.ServiceCategories;
using Home_Service_Finder.Email.Contracts;
using Home_Service_Finder.Email;
using Home_Service_Finder.Locations;
using Home_Service_Finder.ServicesList;
using Home_Service_Finder.RequestServices.ServiceRequest;
using Home_Service_Finder.Roles;
using Home_Service_Finder.RequestServices.ServiceOffers;

namespace Home_Service_Finder.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Home_Service_Finder.Users.ServiceProvider.ServiceProvider> ServiceProviders { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<EmailOTP> Emails { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ServiceList> ServiceLists { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        public DbSet<ServiceRequestServiceList> ServiceRequestServiceLists { get; set; }
        public DbSet<ServiceOffer> ServiceOffers { get; set; }
        public DbSet<ServiceRequestImage> ServiceRequestImages { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
        }
    }
}
