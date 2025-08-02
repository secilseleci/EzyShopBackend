using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Customer;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Concrete;
public class CustomerRepository(ApplicationDbContext context) : BaseRepository<Customer>(context), ICustomerRepository
{
    public async Task<long> CountAsync()
    {
        return await _dataContext.Customers
            .Where(c => !c.IsDeleted && c.IsActive)
            .LongCountAsync();
    }
    public async Task<CustomerSearchResultDto?> GetCustomerDtoByPhoneAsync(string phone)
    {
        var result = await (from c in _dataContext.Customers
                            join u in _dataContext.Users on c.Id equals u.Id
                            where c.Phone == phone
                            select new CustomerSearchResultDto
                            {
                                CustomerId = c.Id,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                Phone = c.Phone,
                                Address = c.Address,
                                CreatedDate = c.CreatedAt,
                                DeletedDate = c.DeletedAt,
                                UpdatedDate = c.UpdatedAt,
                                Email = u.Email!
                            }).FirstOrDefaultAsync();

        return result;
    }
    public async Task<CustomerSearchResultDto?> GetCustomerDtoByEmailAsync(string email)
    {
        var result = await (from u in _dataContext.Users
                            join c in _dataContext.Customers on u.Id equals c.Id
                            where u.Email == email
                            select new CustomerSearchResultDto
                            {
                                CustomerId = c.Id,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                Phone = c.Phone,
                                Address = c.Address,
                                CreatedDate = c.CreatedAt,
                                DeletedDate = c.DeletedAt,
                                UpdatedDate = c.UpdatedAt,
                                Email = u.Email!
                            }).FirstOrDefaultAsync();

        return result;
    }
}