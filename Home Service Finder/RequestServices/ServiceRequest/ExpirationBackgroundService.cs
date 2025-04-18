
using Home_Service_Finder.Data.Contracts;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Home_Service_Finder.RequestServices.ServiceRequest
{
    public class ExpirationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); 
        private readonly TimeSpan _expirationPeriod = TimeSpan.FromMinutes(5); 

        public ExpirationBackgroundService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider
                        .GetRequiredService<IUnitOfWork>();

                    await ExpireOldRequests(dbContext);
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ExpireOldRequests(IUnitOfWork dbContext)
        {
            var now = DateTime.UtcNow;
            var allRequests = await dbContext.ServiceRequests.GetAllAsync();

            foreach (var request in allRequests)
            {
                if (request.Status == "Pending" && request.ExpiresAt <= now)
                {
                    request.Status = "Expired";
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}