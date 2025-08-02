using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Pagination;
using Core.Utilities.Results;
using DataAccess;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.DTOs.Auth;
using Models.DTOs.Seller;
using Models.Entities.Concrete;
using Models.Identity;
using static Models.Entities.Concrete.Seller;

namespace Business.Services.Concrete;

public class SellerService : BaseService, ISellerService
{
    private readonly ApplicationDbContext _context;
    private readonly ISellerRepository _sellerRepo;
    private readonly IShopService _shopService;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    public SellerService(ApplicationDbContext context,
     IShopService shopService,
     ISellerRepository sellerRepo,
     ICurrentUserService currentUserService,
     UserManager<AppUser> userManager,
     RoleManager<AppRole> roleManager,
     IMapper mapper,
     IConfiguration config) : base(mapper, config, currentUserService)
    {
        _context = context;
        _sellerRepo = sellerRepo;
        _shopService = shopService;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    #region Create Seller Registration
    public async Task<IDataResult<Seller>> CreateSellerApplicationAsync(RegisterSellerDto model)
    {
        //Seller Existing Control
        var phoneExists = await _sellerRepo.ExistsAsync(s => s.Phone == model.Phone && !s.IsDeleted);

        if (phoneExists)
        {
            return new ErrorDataResult<Seller>(Messages.AlreadyExistsPhone);
        }

        //Shop Existing Control
        var shopExists = await _shopService.CheckShopExistsAsync(model.ShopName, model.TaxNumber);
        if (!shopExists.Success)
            return new ErrorDataResult<Seller>(shopExists.Message);


        //Transaction Start
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var trx = await _context.Database.BeginTransactionAsync();

            // Create Appuser
            var createAppUserResult = await CreateAppUserAsync(model);

            if (!createAppUserResult.Success)
                throw new Exception(createAppUserResult.Message);

            // Create Seller
            var userId = createAppUserResult.Data.Id;
            var createSellerResult = await CreateSellerAsync(model, userId);

            if (!createSellerResult.Success)
                throw new Exception(createSellerResult.Message);

            // Create Shop
            var sellerId = createSellerResult.Data.Id;
            var createShopResult = await _shopService.CreateShopAsync(model, sellerId);

            if (!createShopResult.Success)
                throw new Exception(createShopResult.Message);

            //Transaction End
            await trx.CommitAsync();

            return new SuccessDataResult<Seller>(createSellerResult.Data);
        });
    }
    #endregion

    #region Activate Seller
    public async Task<IResult> ActivateSellerAsync(Guid sellerId)
    {
        //Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorResult(Messages.LoginUnauthorized);

        //Role check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorResult(Messages.UnauthorizedAccess);

        //Seller check
        var seller = await _sellerRepo.GetAsync(s => s.Id == sellerId);
        if (seller == null)
            return new ErrorResult(Messages.SellerNotFound);

        if (seller.Status != SellerStatus.Pending)
            return new ErrorResult(Messages.InvalidStatus);

        //Transaction Start
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var trx = await _context.Database.BeginTransactionAsync();

            //Activate seller
            seller.Status = SellerStatus.Approved;
            seller.IsActive = true;

            var updateSellerResult = await _sellerRepo.UpdateAsync(seller);
            if (updateSellerResult <= 0)
                throw new Exception(Messages.UpdateError);

            //Activate Shop
            var shopActivatedResult = await _shopService.ActivateShopBySellerIdAsync(sellerId);
            if (!shopActivatedResult.Success)
                throw new Exception(shopActivatedResult.Message);

            // AppUser email confirm
            var existingUser = await _userManager.FindByIdAsync(sellerId.ToString());
            if (existingUser == null)
                throw new Exception(Messages.UserNotFound);

            existingUser.EmailConfirmed = true;

            var updateUserResult = await _userManager.UpdateAsync(existingUser);
            if (!updateUserResult.Succeeded)
            {
                var errors = string.Join(" | ", updateUserResult.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
            //Transaction End
            await trx.CommitAsync();

            return new SuccessResult(Messages.SellerActivated);

        });
    }
    #endregion

    #region Deactivate Seller
    public async Task<IResult> DeactivateSellerAsync(Guid sellerId)
    {
        //Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorResult(Messages.LoginUnauthorized);

        //Role check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorResult(Messages.UnauthorizedAccess);

        //Seller check
        var seller = await _sellerRepo.GetAsync(s => s.Id == sellerId);
        if (seller == null)
            return new ErrorResult(Messages.SellerNotFound);

        if (seller.Status != SellerStatus.Pending)
            return new ErrorResult(Messages.InvalidStatus);

        //Transaction Start
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var trx = await _context.Database.BeginTransactionAsync();

            //Delete Shop
            var shopDeleteResult = await _shopService.DeleteShopAsync(sellerId);
            if (!shopDeleteResult.Success)
                throw new Exception(shopDeleteResult.Message);

            //Delete Seller
            seller.Status = SellerStatus.Rejected;
            seller.IsActive = false;
            seller.IsDeleted = true;

            var updateSellerResult = await _sellerRepo.UpdateAsync(seller);
            if (updateSellerResult <= 0)
                throw new Exception(Messages.UpdateError);

            //Delete Appuser
            var user = await _userManager.FindByIdAsync(sellerId.ToString());
            if (user != null)
            {
                user.IsDeleted = true;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new Exception(Messages.DeleteError);
            }

            //Transaction End
            await trx.CommitAsync();

            return new SuccessResult(Messages.SellerDeactivated);

        });
    }
    #endregion

    #region Ban Seller
    public async Task<IResult> BanSellerAsync(Guid sellerId)
    {
        //Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorResult(Messages.LoginUnauthorized);

        //Role check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorResult(Messages.UnauthorizedAccess);

        //Seller check
        var seller = await _sellerRepo.GetAsync(s => s.Id == sellerId);
        if (seller == null)
            return new ErrorResult(Messages.SellerNotFound);

        if (seller.Status != SellerStatus.Approved)
            return new ErrorResult(Messages.InvalidStatus);

        //Transaction Start
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var trx = await _context.Database.BeginTransactionAsync();

            //Deactivate Shop
            var shopDeactivateResult = await _shopService.DeactivateShopBySellerIdAsync(sellerId);
            if (!shopDeactivateResult.Success)
                throw new Exception(shopDeactivateResult.Message);

            //Deactivate Seller
            seller.Status = SellerStatus.Banned;
            seller.IsActive = false;

            var updateSellerResult = await _sellerRepo.UpdateAsync(seller);
            if (updateSellerResult <= 0)
                throw new Exception(Messages.UpdateError);

            //Transaction End
            await trx.CommitAsync();

            return new SuccessResult(Messages.SellerBanned);

        });
    }
    #endregion

    #region List Seller
    public async Task<DataResult<PaginatedList<SellerListItemDto>>> GetFilteredSellerListAsync(SellerFilterDto filter)
    {
        // Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<PaginatedList<SellerListItemDto>>(Messages.LoginUnauthorized);

        // Role check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorDataResult<PaginatedList<SellerListItemDto>>(Messages.UnauthorizedAccess);

        // Repository
        var result = await _sellerRepo.GetFilteredSellerListAsync(filter);

        if (result == null || !result.Items.Any())
            return new ErrorDataResult<PaginatedList<SellerListItemDto>>(message: Messages.EmptyEntityList);

        return new SuccessDataResult<PaginatedList<SellerListItemDto>>(data: result);
    }
    #endregion

    #region Private Methods
    private async Task<DataResult<AppUser>> CreateAppUserAsync(RegisterSellerDto model)
    {
        var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Phone);

        if (existingUser != null && !existingUser.IsDeleted)
        {
            return new ErrorDataResult<AppUser>(message: Messages.AlreadyExistsPhone);
        }

        var user = new AppUser
        {
            Email = model.Email,
            UserName = model.FullName,
            PhoneNumber = model.Phone
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errorMessages = string.Join(" | ", result.Errors.Select(e => e.Description));
            return new ErrorDataResult<AppUser>(message: errorMessages);
        }

        if (!await _roleManager.RoleExistsAsync(CustomRoles.Seller))
            await _roleManager.CreateAsync(new AppRole { Name = CustomRoles.Seller });

        await _userManager.AddToRoleAsync(user, CustomRoles.Seller);

        return new SuccessDataResult<AppUser>(data: user);
    }
    private async Task<DataResult<Seller>> CreateSellerAsync(RegisterSellerDto model, Guid userId)
    {
        var existingSeller = await _sellerRepo.GetByIdAsync(userId);

        if (existingSeller != null)
        {
            return new ErrorDataResult<Seller>(Messages.AlreadyExistsSeller);
        }

        var seller = Mapper.Map<Seller>(model);

        seller.Id = userId;
        seller.CreatedBy = model.FullName;
        seller.IsActive = false;

        var createSellerResult = await _sellerRepo.CreateAsync(seller);

        if (createSellerResult <= 0)
        {
            return new ErrorDataResult<Seller>(message: Messages.CreateError);
        }

        return new SuccessDataResult<Seller>(data: seller);
    }
    #endregion

}