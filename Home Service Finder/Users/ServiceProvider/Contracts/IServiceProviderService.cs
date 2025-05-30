﻿using Home_Service_Finder.Users.ServiceProvider.Dtos;

namespace Home_Service_Finder.Users.ServiceProvider.Contracts
{
    public interface IServiceProviderService
    {
        Task<APIResponse> AddServiceProviderAsync(ServiceProviderRequestDto serviceProviderRequestDto);
        Task<APIResponse> GetAllServiceProviderAsync();
        Task<APIResponse> GetServiceProviderById(Guid id);
        Task<APIResponse> DeleteServiceProviderAsync(Guid id);
        Task<APIResponse> UpdateServiceProvider(Guid id, ServiceProviderUpdateRequestDto serviceProviderUpdateRequestDto);
        Task<APIResponse> GetServiceProviderStatisticsAsync(Guid providerId);
        Task<APIResponse> GetRevenueTimeSeriesAsync(Guid providerId, string groupBy = "month");


    }
}
