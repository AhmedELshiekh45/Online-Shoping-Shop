using AutoMapper;
using BussinessLayer.ViewModels;
using DataAccess.Model;

namespace Presentaion.Mapping
{
    public class CartItemProfile : Profile
    {
        public CartItemProfile()
        {
            CreateMap<CartItemVm, CartItem>()
              .ForPath(des => des.Product.Name, so => so.MapFrom(p => p.ProductName))
              .ForPath(des => des.Product.UnitPrice, so => so.MapFrom(p => p.Price))
              .ReverseMap();
            CreateMap<CartItem, OrderItems>()
                .ReverseMap();
        }
    }
}
