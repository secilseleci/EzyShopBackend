using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities.Results;
using DataAccess;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.DTOs.Auth;
using Models.DTOs.Customer;
using Models.Entities.Concrete;
using Models.Identity;

namespace Business.Services.Concrete;
public class CustomerService : BaseService, ICustomerService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public CustomerService(ApplicationDbContext context,
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
        _context = context;
    }
    public async Task<IResult> RegisterCustomerAsync(RegisterCustomerDto model)
    {
        // AppUser check
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null && !existingUser.IsDeleted)
            return new ErrorResult(Messages.AlreadyExistsEmail);

        //Transaction Start
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var trx = await _context.Database.BeginTransactionAsync();

            // Create AppUser 
            var user = new AppUser
            {
                Email = model.Email,
                UserName = model.FullName,
                PhoneNumber = model.Phone,
                EmailConfirmed = true
            };

            var createUserResult = await _userManager.CreateAsync(user, model.Password);
            if (!createUserResult.Succeeded)
            {
                var errors = string.Join(" | ", createUserResult.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            // Role assignment
            if (!await _roleManager.RoleExistsAsync(CustomRoles.Customer))
                await _roleManager.CreateAsync(new AppRole { Name = CustomRoles.Customer });

            await _userManager.AddToRoleAsync(user, CustomRoles.Customer);

            // Create Customer 
            var customer = Mapper.Map<Customer>(model);
            customer.Id = user.Id;
            customer.CreatedBy = model.FullName;

            var createResult = await _customerRepo.CreateAsync(customer);
            if (createResult <= 0)
                throw new Exception(Messages.CreateError);

            //Transaction End
            await trx.CommitAsync();

            return new SuccessResult(Messages.CreateSuccess);
        });
    }
    public async Task<IDataResult<CustomerSearchResultDto>> GetCustomerBySearchAsync(CustomerSearchDto model)
    {
        //Login Check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<CustomerSearchResultDto>(Messages.LoginUnauthorized);

        //Role Check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorDataResult<CustomerSearchResultDto>(Messages.UnauthorizedAccess);

        //Customer Check
        CustomerSearchResultDto? dto = null;

        if (!string.IsNullOrWhiteSpace(model.Phone))
            dto = await _customerRepo.GetCustomerDtoByPhoneAsync(model.Phone);
        else if (!string.IsNullOrWhiteSpace(model.Email))
            dto = await _customerRepo.GetCustomerDtoByEmailAsync(model.Email);
        else
            return new ErrorDataResult<CustomerSearchResultDto>(Messages.NoFilter);

        if (dto == null)
            return new ErrorDataResult<CustomerSearchResultDto>(Messages.CustomerNotFound);

        return new SuccessDataResult<CustomerSearchResultDto>(dto);
    }
    public async Task<IResult> DeleteCustomerByAdminAsync(Guid customerId)
    {
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorResult(Messages.UnauthorizedAccess);

        return await SoftDeleteCustomerInternalAsync(customerId);
    }
    public async Task<IResult> DeleteOwnCustomerAccountAsync()
    {
        if (!CurrentUserService.UserId.HasValue || CurrentUserService.Role != CustomRoles.Customer)
            return new ErrorResult(Messages.UnauthorizedAccess);

        return await SoftDeleteCustomerInternalAsync(CurrentUserService.UserId.Value);
    }
    public async Task<IDataResult<long>> CountAsync()
    {
        var count = await _customerRepo.CountAsync();
        return new SuccessDataResult<long>(count);
    }
    public async Task<IDataResult<CustomerSearchResultDto>> GetOwnProfileAsync()
    {
        if (!CurrentUserService.UserId.HasValue || CurrentUserService.Role != CustomRoles.Customer)
            return new ErrorDataResult<CustomerSearchResultDto>(Messages.UnauthorizedAccess);

        var userId = CurrentUserService.UserId.Value;

        var dto = await _customerRepo.GetOwnProfileAsync(userId);
        if (dto == null)
            return new ErrorDataResult<CustomerSearchResultDto>(Messages.CustomerNotFound);

        return new SuccessDataResult<CustomerSearchResultDto>(dto);
    }
    private async Task<IResult> SoftDeleteCustomerInternalAsync(Guid customerId)
    {
        //Check customer
        var exists = await _customerRepo.ExistsAsync(c => c.Id == customerId && !c.IsDeleted);
        if (!exists)
            return new ErrorResult(Messages.CustomerNotFound);

        //Transaction Start
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var trx = await _context.Database.BeginTransactionAsync();

            //Delete Customer
            var deleteResult = await _customerRepo.SoftDeleteAsync(customerId);
            if (deleteResult <= 0)
                throw new Exception(Messages.DeleteError);

            //Delete Appuser
            var user = await _userManager.FindByIdAsync(customerId.ToString());
            if (user != null)
            {
                user.IsDeleted = true;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new Exception(Messages.DeleteError);
            }

            await trx.CommitAsync();
            return new SuccessResult(Messages.DeleteSuccess);
        });
    }
}