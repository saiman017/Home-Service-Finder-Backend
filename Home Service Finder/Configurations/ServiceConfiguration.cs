using Home_Service_Finder.Data;
using Microsoft.EntityFrameworkCore;

namespace Home_Service_Finder.Configurations
{
    public static class ServiceConfiguration
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
               options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
