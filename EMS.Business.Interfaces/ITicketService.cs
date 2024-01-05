using EMS.Business.Interfaces;
using EMS.Business.Models;
using EMS.Data.Models;

public interface ITicketService : IGenericCrudService<Ticket, TicketModel>
{
    Task<IQueryable<TicketModel>> GetTicketsByEventId(int eventId);
    Task<IQueryable<TicketModel>> GetTicketsByUserId(int eventId);
}
