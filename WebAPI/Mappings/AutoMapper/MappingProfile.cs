using AutoMapper;
using Models.DTOs.Auth;
using Models.DTOs.Category;
using Models.DTOs.Customer;
using Models.DTOs.Product;
using Models.DTOs.SubscriptionPlan;
using Models.Entities.Concrete;

namespace WebAPI.Mappings.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region Seller for Register
        CreateMap<RegisterSellerDto, Seller>()
            .AfterMap((src, dest) =>
         {
             var names = src.FullName.Split(' ');
             dest.FirstName = names[0];
             dest.LastName = names.Length > 1
                             ? string.Join(" ", names.Skip(1))
                             : "";
         });


        CreateMap<Seller, RegisterSellerDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));


        CreateMap<RegisterSellerDto, Shop>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ShopName));

        #endregion

        #region Customer for Register
        CreateMap<RegisterCustomerDto, Customer>()
            .AfterMap((src, dest) =>
        {
            var names = src.FullName.Split(' ');
            dest.FirstName = names[0];
            dest.LastName = names.Length > 1
                            ? string.Join(" ", names.Skip(1))
                            : "";
        });

        CreateMap<Customer, RegisterCustomerDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));
        #endregion

        #region Category
        CreateMap<Category, CategoryBasicDto>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => (src.Name ?? string.Empty).Trim())).ReverseMap();
        #endregion

        #region Product 
        CreateMap<Product, ProductBasicDto>().ReverseMap();

        CreateMap<Product, CreateProductDto>().ReverseMap();

        CreateMap<Product, UpdateProductDto>().ReverseMap();
        #endregion

        #region Customer Search  
        CreateMap<Customer, CustomerSearchResultDto>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id));
        #endregion

        #region SubscriptionPlan
        CreateMap<SubscriptionPlan, CreateSubscriptionPlanDto>().ReverseMap();
        #endregion

    }
}