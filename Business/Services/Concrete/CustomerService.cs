using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Pagination;
using Core.Utilities.Results;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models.Entities.Concrete;
using Models.Identity;
using Models.ViewModels.Auth;
using Models.ViewModels.Customer;

namespace Business.Services.Concrete;

public class CustomerService : BaseService, ICustomerService
{
    private readonly ICustomerRepository _customerRepo;
    public CustomerService(
      IMapper mapper,
      IConfiguration config,
      ICurrentUserService currentUserService,
      UserManager<AppUser> userManager,
      RoleManager<AppRole> roleManager,
      ICustomerRepository customerRepo)
   : base(mapper, config, currentUserService)
    {
        _customerRepo = customerRepo;
    }

    public async Task<decimal> CountAsync()
    {
        return await _customerRepo.CountAsync();
    }
    public async Task<IResult> CreateCustomerAsync(Guid userId, RegisterCustomerViewModel model)
    {
        var existingCustomer = await _customerRepo.GetByIdAsync(userId);
        if (existingCustomer != null)
        {
            return new ErrorResult(Messages.AlreadyExistsCustomer);
        }

        var customer = Mapper.Map<Customer>(model);
        customer.Id = userId;
        customer.CreatedBy = model.FullName;

        var createResult = await _customerRepo.CreateAsync(customer);
        return createResult > 0
            ? new SuccessResult(Messages.CreateSuccess)
            : new ErrorResult(Messages.CreateError);
    }
    public async Task<IResult> DeleteCustomerAsync(Guid customerId)
    {
        if (!await _customerRepo.ExistsAsync(c => c.Id == customerId && !c.IsDeleted))
            return new ErrorResult(Messages.CustomerNotFound);

        var deleteResult = await _customerRepo.SoftDeleteAsync(customerId);

        return deleteResult > 0
            ? new SuccessResult(Messages.DeleteSuccess)
        : new ErrorResult(Messages.DeleteError);
    }
    public async Task<IDataResult<PaginatedList<CustomerListViewModel>>> GetPaginatedCustomerListAsync(string? searchTerm, int page, int pageSize)
    {
        var paginated = await _customerRepo.GetPaginatedCustomerDtosAsync(searchTerm, page, pageSize);

        if (!paginated.Items.Any())
            return new ErrorDataResult<PaginatedList<CustomerListViewModel>>(Messages.EmptyEntityList);

        return new SuccessDataResult<PaginatedList<CustomerListViewModel>>(paginated);
    }
}