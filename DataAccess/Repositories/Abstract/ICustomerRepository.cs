using Core.Pagination;
using Models.Entities.Concrete;
using Models.ViewModels.Customer;

namespace DataAccess.Repositories.Abstract;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<decimal> CountAsync();
    Task<PaginatedList<CustomerListViewModel>> GetPaginatedCustomerDtosAsync(
   string? searchTerm, int page, int pageSize);
}
