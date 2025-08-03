using Models.DTOs.Customer;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Abstract;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<long> CountAsync();
    Task<CustomerSearchResultDto?> GetCustomerDtoByEmailAsync(string email);
    Task<CustomerSearchResultDto?> GetCustomerDtoByPhoneAsync(string phone);
    Task<CustomerSearchResultDto?> GetOwnProfileAsync(Guid customerId);

}
