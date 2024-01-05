using AutoMapper;
using EMS.Business.Models;
using EMS.Data.Interfaces;
using EMS.Data.Models;
namespace EMS.Business.DataService
{
    public class TicketService : GenericCrudService<Ticket, TicketModel>, ITicketService
    {
        public TicketService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<IQueryable<TicketModel>> GetTicketsByUserId(int userId)
        {
            var tickets = await _repository.GetAll();
            var ticketsQuery = tickets.Where(t => t.UserId == userId).Select(t => _mapper.Map<TicketModel>(t));
            return ticketsQuery.AsQueryable();
        }

        public async Task<IQueryable<TicketModel>> GetTicketsByEventId(int eventId)
        {
            var tickets = await _repository.GetAll();
            var ticketsQuery = tickets.Where(t => t.EventId == eventId).Select(t => _mapper.Map<TicketModel>(t));
            return ticketsQuery.AsQueryable();
        }
    }
}
