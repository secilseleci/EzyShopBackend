using Core.Pagination;
using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.Entities.Concrete;
using Models.ViewModels.Customer;


namespace DataAccess.Repositories.Concrete;

public class CustomerRepository(ApplicationDbContext context) : BaseRepository<Customer>(context), ICustomerRepository
{
    public async Task<decimal> CountAsync()
    {
        return await _dataContext.Customers
            .Where(c => !c.IsDeleted)
            .LongCountAsync();
    }

    public async Task<PaginatedList<CustomerListViewModel>> GetPaginatedCustomerDtosAsync(
    string? searchTerm, int page, int pageSize)
    {
        var query = from customer in _dataContext.Customers
                    join user in _dataContext.Users
                     on customer.Id equals user.Id
                    where !customer.IsDeleted &&
                          (string.IsNullOrEmpty(searchTerm) ||
                           (customer.FirstName + " " + customer.LastName).Contains(searchTerm) ||
                           customer.Address.Contains(searchTerm))
                    select new CustomerListViewModel
                    {
                        Id = customer.Id,
                        FullName = customer.FirstName + " " + customer.LastName,
                        Email = user.Email!,
                        Address = customer.Address,
                        Phone = customer.Phone
                    };

        return await GetPaginatedAsync(query, page, pageSize);
    }

}