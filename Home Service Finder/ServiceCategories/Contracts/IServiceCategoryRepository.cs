using Home_Service_Finder.Data.Contracts;

namespace Home_Service_Finder.ServiceCategories.Contracts
{
    //public interface IServiceCategoryRepository : IGenericRepository<ServiceCategory>
    //{

    //}

    public interface IServiceCategoryRepository : IGenericRepository<ServiceCategory>
    {

        Task<ServiceCategory> GetByServiceCategoryName(string name);
        Task<ServiceCategory> FindByNameIncludingDeletedAsync(string name);
    }
}
