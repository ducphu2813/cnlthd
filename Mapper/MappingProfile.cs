using APIApplication.DTO;
using APIApplication.DTO.Invoice;
using APIApplication.DTO.InvoiceDetail;
using APIApplication.DTO.Users;
using APIApplication.Model;
using AutoMapper;

namespace APIApplication.Mapper;

public class MappingProfile : Profile
{
    
    public MappingProfile()
    {
        //mapper của product
        CreateMap<Product, SaveProductDTO>().ReverseMap();
        CreateMap<Product, ProductDTO>().ReverseMap();
        
        //mapper của users
        CreateMap<Users, SaveUserDTO>().ReverseMap();
        CreateMap<Users, UserDTO>().ReverseMap();
        
        //mapper của invoice
        CreateMap<Invoice, SaveInvoiceDTO>().ReverseMap();
        CreateMap<Invoice, InvoiceDTO>()
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users))
            .ForMember(dest => dest.InvoiceDetails, opt => opt.MapFrom(src => src.InvoiceDetails));
        
        //mapper ủa invoice detail
        CreateMap<InvoiceDetail, SaveInvoiceDetailDTO>().ReverseMap();
        CreateMap<InvoiceDetail, InvoiceDetailDTO>()
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));
    }
    
}