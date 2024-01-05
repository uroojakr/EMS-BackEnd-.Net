using AutoMapper;
using EMS.Business.Models;
using EMS.Data.Models;

public class BusinessMappingProfile : Profile
{
    public BusinessMappingProfile()
    {
        CreateMap<Events, EventsModel>().ReverseMap();
        CreateMap<Review, ReviewModel>().ReverseMap();
        CreateMap<Ticket, TicketModel>().ReverseMap();
        CreateMap<User, UserModel>().ReverseMap();
        CreateMap<Vendor, VendorModel>().ReverseMap();
        CreateMap<VendorEvent, VendorEventModel>().ReverseMap();

    }
}
