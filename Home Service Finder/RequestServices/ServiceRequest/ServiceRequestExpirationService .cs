using Home_Service_Finder.Data.Contracts;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Home_Service_Finder.RequestServices.ServiceRequest
{
    public class ServiceRequestExpirationService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _requestExpiration = TimeSpan.FromMinutes(20);

        public ServiceRequestExpirationService(IServiceProvider services)
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

                    await ExpireOldServiceRequests(dbContext);
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ExpireOldServiceRequests(IUnitOfWork dbContext)
        {
            var now = DateTime.UtcNow;
            var pendingRequests = (await dbContext.ServiceRequests.GetAllAsync())
                .Where(r => r.Status == "Pending" && r.ExpiresAt <= now)
                .ToList();

            foreach (var request in pendingRequests)
            {
                request.Status = "Expired";
            }

            if (pendingRequests.Any())
            {
                await dbContext.SaveChangesAsync();
            }
        }
    }
}