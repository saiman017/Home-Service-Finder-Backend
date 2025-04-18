namespace Home_Service_Finder.Data.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> DeleteAsync(Guid id);
        Task<T> AddAsync(T entity);
        T UpdateAsync( T entity);
    }
}
