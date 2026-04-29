using AutoMapper;
using Shop.Domain.Entities;
using Shop.Models;
using Shop.ViewModels;

namespace Shop.Database
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DataRentApart, RentApartViewModel>();
            //CreateMap<DataRentApart, RentApartViewModel>()
            //  .ForMember(dest => dest. , opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
        }
    }

}
