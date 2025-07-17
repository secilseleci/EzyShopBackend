using AutoMapper;
using Models.Entities.Concrete;
using Models.ViewModels.Auth;
using Models.ViewModels.Category;
using Models.ViewModels.Customer;
using Models.ViewModels.Product;


namespace WebUI.Mappings.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region Seller for Register
        CreateMap<RegisterSellerViewModel, Seller>()
            .AfterMap((src, dest) =>
         {
             var names = src.FullName.Split(' ');
             dest.FirstName = names[0];
             dest.LastName = names.Length > 1
                             ? string.Join(" ", names.Skip(1))
                             : "";
         });


        CreateMap<Seller, RegisterSellerViewModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));

        #endregion

        #region Shop for Register
        CreateMap<RegisterSellerViewModel, Shop>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ShopName))
            .ForMember(dest => dest.SellerId, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore());
        #endregion

        #region Customer for Register
        CreateMap<RegisterCustomerViewModel, Customer>()
        .AfterMap((src, dest) =>
        {
            var names = src.FullName.Split(' ');
            dest.FirstName = names[0];
            dest.LastName = names.Length > 1
                            ? string.Join(" ", names.Skip(1))
                            : "";
        });
        CreateMap<Customer, RegisterCustomerViewModel>()
        .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));
        #endregion

        #region Category
        CreateMap<Category, CategoryViewModel>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => (src.Name ?? string.Empty).Trim())).ReverseMap();
        #endregion

        #region Product 
        CreateMap<CreateProductViewModel, Product>();
      
       CreateMap<Product, UpdateProductViewModel>().ReverseMap();
  #endregion
        //CreateMap<Product, ProductSellerViewModel>();

        //CreateMap<Product, ProductCustomerViewModel>();
        //CreateMap<Product, ProductFilterViewModel>();
        //CreateMap<Product, ProductDetailViewModel>();


        //#endregion



        //  #region Shop  
        /// CreateMap<Shop, ShopViewModel>().ReverseMap();

        //  #endregion



        #region Customer for List
        CreateMap<Customer, CustomerListViewModel>()
         .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));
        #endregion

        //#region Seller for Profile
        //CreateMap<Seller, SellerViewModel>()
        //  .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}".Trim()))
        //  .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));



        //#endregion








    }
}