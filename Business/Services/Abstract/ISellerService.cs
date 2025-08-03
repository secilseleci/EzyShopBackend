using Core.Pagination;
using Core.Utilities.Results;
using Models.DTOs.Auth;
using Models.DTOs.Seller;
using Models.Entities.Concrete;

namespace Business.Services.Abstract;

public interface ISellerService
{
    Task<IDataResult<Seller>> CreateSellerApplicationAsync(RegisterSellerDto model);
    Task<IResult> ActivateSellerAsync(Guid sellerId);
    Task<IResult> DeactivateSellerAsync(Guid sellerId);
    Task<IResult> BanSellerAsync(Guid sellerId);
    Task<DataResult<PaginatedList<SellerListItemDto>>> GetFilteredSellerListAsync(SellerFilterDto filter);
    Task<IDataResult<SellerProfileDto>> GetOwnProfileAsync();
}
