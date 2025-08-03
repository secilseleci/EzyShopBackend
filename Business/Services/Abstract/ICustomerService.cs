using Core.Utilities.Results;
using Models.DTOs.Auth;
using Models.DTOs.Customer;

namespace Business.Services.Abstract;
public interface ICustomerService
{
    Task<IResult> RegisterCustomerAsync(RegisterCustomerDto model);
    Task<IDataResult<CustomerSearchResultDto>> GetCustomerBySearchAsync(CustomerSearchDto model);
    Task<IResult> DeleteCustomerByAdminAsync(Guid customerId);
    Task<IResult> DeleteOwnCustomerAccountAsync();
    Task<IDataResult<long>> CountAsync();
    Task<IDataResult<CustomerSearchResultDto>> GetOwnProfileAsync();
}