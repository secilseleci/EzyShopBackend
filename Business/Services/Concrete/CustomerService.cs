using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Pagination;
using Core.Utilities.Results;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models.DTOs.Auth;
using Models.Entities.Concrete;
using Models.Identity;
using Models.ViewModels.Customer;

namespace Business.Services.Concrete;

public class CustomerService : BaseService, ICustomerService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
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
        _userManager = userManager;
        _roleManager = roleManager;
    }


    public async Task<IResult> RegisterCustomerAsync(RegisterCustomerDto model)
    {
        // 1. AppUser kontrolü
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null && !existingUser.IsDeleted)
            return new ErrorResult(Messages.AlreadyExistsEmail);

        // 2. AppUser oluşturma
        var user = new AppUser
        {
            Email = model.Email,
            UserName = model.Email,
            PhoneNumber = model.Phone,
            EmailConfirmed = true
        };

        var createUserResult = await _userManager.CreateAsync(user, model.Password);
        if (!createUserResult.Succeeded)
        {
            var errors = string.Join(" | ", createUserResult.Errors.Select(e => e.Description));
            return new ErrorResult(errors);
        }

        // 3. Rol kontrol & atama
        if (!await _roleManager.RoleExistsAsync(CustomRoles.Customer))
            await _roleManager.CreateAsync(new AppRole { Name = CustomRoles.Customer });

        await _userManager.AddToRoleAsync(user, CustomRoles.Customer);

        // 4. Customer kontrol
        var existingCustomer = await _customerRepo.GetByIdAsync(user.Id);
        if (existingCustomer != null)
        {
            await _userManager.DeleteAsync(user); // AppUser'ı manuel sil
            return new ErrorResult(Messages.AlreadyExistsCustomer);
        }

        // 5. Customer oluşturma
        var customer = Mapper.Map<Customer>(model);
        customer.Id = user.Id;
        customer.CreatedBy = model.FullName;

        var createResult = await _customerRepo.CreateAsync(customer);
        if (createResult <= 0)
        {
            await _userManager.DeleteAsync(user); // AppUser'ı manuel silme
            return new ErrorResult(Messages.CreateError);
        }

        return new SuccessResult(Messages.CreateSuccess);
    }
    public async Task<decimal> CountAsync()
    {
        return await _customerRepo.CountAsync();
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