using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home_Service_Finder.Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Home_Service_Finder.AdminDashboard
{
    [ApiController]
    [Route("api/admin/dashboard")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IUnitOfWork _db;
        public AdminDashboardController(IUnitOfWork db) => _db = db;

        [HttpGet("summary")]
        public async Task<SummaryDto> GetSummary()
        {
            var totalRequests = await _db.ServiceRequests.Entities.CountAsync();
            var totalRevenue = await _db.ServiceOffers.Entities
                                        .Where(o => o.PaymentStatus)
                                        .SumAsync(o => o.OfferedPrice);
            return new SummaryDto(totalRequests, totalRevenue);
        }

        
        [HttpGet("requests")]
        public async Task<IEnumerable<TimeSeriesDto>> GetRequests([FromQuery] string groupBy = "day")
        {

            var requests = await _db.ServiceRequests.Entities.ToListAsync();

            switch (groupBy.ToLower())
            {
                case "day":
                    {
                        return requests
                            .GroupBy(r => r.CreatedAt.Date)  
                            .Select(g => new TimeSeriesDto(
                                g.Key.ToString("yyyy-MM-dd"),
                                g.Count()
                            ))
                            .OrderBy(x => x.Period)
                            .ToList();
                    }

                case "week":
                    {
                        return requests
                            .GroupBy(r => new {
                                Year = r.CreatedAt.Year,
                                Week = (r.CreatedAt.DayOfYear - 1) / 7 + 1
                            })
                            .Select(g => new TimeSeriesDto(
                                $"{g.Key.Year:0000}-W{g.Key.Week:00}",
                                g.Count()
                            ))
                            .OrderBy(x => x.Period)
                            .ToList();
                    }

                case "month":
                    {
                        return requests
                            .GroupBy(r => new {
                                Year = r.CreatedAt.Year,
                                Month = r.CreatedAt.Month
                            })
                            .Select(g => new TimeSeriesDto(
                                new DateTime(g.Key.Year, g.Key.Month, 1).ToString("yyyy-MM"),
                                g.Count()
                            ))
                            .OrderBy(x => x.Period)
                            .ToList();
                    }

                default:
                    throw new ArgumentException("groupBy must be 'day', 'week' or 'month'");
            }
        }

        // 3) Revenue over time, grouped by day/week/month
        [HttpGet("revenue")]
        public async Task<IEnumerable<RevenueDto>> GetRevenue([FromQuery] string groupBy = "month")
        {
            var paidOffers = await _db.ServiceOffers.Entities
                .Where(o => o.PaymentStatus)
                .ToListAsync();

            switch (groupBy.ToLower())
            {
                case "day":
                    {
                        return paidOffers
                            .GroupBy(o => o.SentAt.Date) 
                            .Select(g => new RevenueDto(
                                g.Key.ToString("yyyy-MM-dd"),
                                g.Sum(o => o.OfferedPrice)
                            ))
                            .OrderBy(x => x.Period)
                            .ToList();
                    }

                case "week":
                    {
                        return paidOffers
                            .GroupBy(o => new {
                                Year = o.SentAt.Year,
                                Week = (o.SentAt.DayOfYear - 1) / 7 + 1  
                            })
                            .Select(g => new RevenueDto(
                                $"{g.Key.Year:0000}-W{g.Key.Week:00}",
                                g.Sum(o => o.OfferedPrice)
                            ))
                            .OrderBy(x => x.Period)
                            .ToList();
                    }

                case "month":
                    {
                        return paidOffers
                            .GroupBy(o => new {
                                Year = o.SentAt.Year,
                                Month = o.SentAt.Month
                            })
                            .Select(g => new RevenueDto(
                                new DateTime(g.Key.Year, g.Key.Month, 1).ToString("yyyy-MM"),
                                g.Sum(o => o.OfferedPrice)
                            ))
                            .OrderBy(x => x.Period)
                            .ToList();
                    }

                default:
                    throw new ArgumentException("groupBy must be 'day', 'week' or 'month'");
            }
        }
        [HttpGet("revenue/provider/{providerId:guid}")]
        public async Task<IEnumerable<RevenueDto>> GetRevenueByProvider(Guid providerId,
   [FromQuery] string groupBy = "month"
)
        {
            var query = _db.ServiceOffers.Entities
                           .Where(o => o.PaymentStatus && o.ServiceProviderId == providerId);

            var paidOffers = await query.ToListAsync();

            switch (groupBy.ToLower())
            {
                case "day":
                    return paidOffers
                        .GroupBy(o => o.SentAt.Date)
                        .Select(g => new RevenueDto(
                            g.Key.ToString("yyyy-MM-dd"),
                            g.Sum(o => o.OfferedPrice)
                        ))
                        .OrderBy(x => x.Period)
                        .ToList();

                case "week":
                    return paidOffers
                        .GroupBy(o => new {
                            Year = o.SentAt.Year,
                            Week = (o.SentAt.DayOfYear - 1) / 7 + 1
                        })
                        .Select(g => new RevenueDto(
                            $"{g.Key.Year:0000}-W{g.Key.Week:00}",
                            g.Sum(o => o.OfferedPrice)
                        ))
                        .OrderBy(x => x.Period)
                        .ToList();

                default: // month
                    return paidOffers
                        .GroupBy(o => new {
                            Year = o.SentAt.Year,
                            Month = o.SentAt.Month
                        })
                        .Select(g => new RevenueDto(
                            new DateTime(g.Key.Year, g.Key.Month, 1)
                                .ToString("yyyy-MM"),
                            g.Sum(o => o.OfferedPrice)
                        ))
                        .OrderBy(x => x.Period)
                        .ToList();
            }
        }

        // 4) Top Providers by completed jobs
        [HttpGet("top-providers")]
        public async Task<IEnumerable<ProviderPerformanceDto>> GetTopProviders([FromQuery] int take = 5)
        {
            var stats = await _db.ServiceOffers.Entities
                .Where(o => o.Status == "Accepted" && o.PaymentStatus)
                .GroupBy(o => o.ServiceProviderId)
                .Select(g => new { ProviderId = g.Key, CompletedJobs = g.Count() })
                .OrderByDescending(x => x.CompletedJobs)
                .Take(take)
                .ToListAsync();

            var providerIds = stats.Select(x => x.ProviderId).ToList();

            var providers = await _db.ServiceProviders.Entities
                .Include(p => p.User)
                  .ThenInclude(u => u.UserDetail)
                .Where(p => providerIds.Contains(p.Id))
                .ToListAsync();

            return stats.Select(x =>
            {
                var p = providers.First(pv => pv.Id == x.ProviderId);
                var name = $"{p.User.UserDetail.FirstName} {p.User.UserDetail.LastName}";
                return new ProviderPerformanceDto(x.ProviderId, name, x.CompletedJobs);
            });
        }

        // 5) Request Status Breakdown
        [HttpGet("status-breakdown")]
        public async Task<IEnumerable<StatusBreakdownDto>> GetStatusBreakdown()
        {
            return await _db.ServiceRequests.Entities
                        .GroupBy(r => r.Status)
                        .Select(g => new StatusBreakdownDto(g.Key, g.Count()))
                        .ToListAsync();
        }
    }

    public record SummaryDto(int TotalRequests, decimal TotalRevenue);
    public record TimeSeriesDto(string Period, int Count);
    public record RevenueDto(string Period, decimal Amount);
    public record ProviderPerformanceDto(Guid ProviderId, string Name, int CompletedJobs);
    public record StatusBreakdownDto(string Status, int Count);
}