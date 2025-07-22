using Core.Pagination;
using Core.Utilities.Results;
using Models.DTOs.Auth;
using Models.ViewModels.Customer;

namespace Business.Services.Abstract;

public interface ICustomerService
{
    Task<IResult> RegisterCustomerAsync(RegisterCustomerDto model);
    Task<IResult> DeleteCustomerAsync(Guid customerId);
    Task<IDataResult<PaginatedList<CustomerListViewModel>>> GetPaginatedCustomerListAsync(string? searchTerm, int page, int pageSize);
    Task<decimal> CountAsync();
}