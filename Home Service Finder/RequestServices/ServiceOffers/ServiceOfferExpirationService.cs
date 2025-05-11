using Home_Service_Finder.Data.Contracts;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Home_Service_Finder.RequestServices.ServiceOffers
{
    public class ServiceOfferExpirationService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _offerExpiration = TimeSpan.FromMinutes(5);

        public ServiceOfferExpirationService(IServiceProvider services)
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

                    await ExpireOldServiceOffers(dbContext);
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ExpireOldServiceOffers(IUnitOfWork dbContext)
        {
            var now = DateTime.UtcNow;
            var pendingOffers = (await dbContext.ServiceOffers.GetAllAsync())
                .Where(o => o.Status == "Pending" && o.ExpiresAt <= now)
                .ToList();

            foreach (var offer in pendingOffers)
            {
                offer.Status = "Expired";
            }

            if (pendingOffers.Any())
            {
                await dbContext.SaveChangesAsync();
            }
        }
    }
}