using AutoMapper;
using EMS.Business.Interfaces;
using EMS.Business.Models;
using EMS.Data.Interfaces;
using EMS.Data.Models;

namespace EMS.Business.DataService
{
    public class EventService : GenericCrudService<Events, EventsModel>, IEventsService
    {
        public EventService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<EventsModel>> GetEventsByLocation(string location)
        {
            var events = await _repository.GetAll();
            var eventsQuery = events.Where(e => e.Location == location);
            return _mapper.Map<IEnumerable<EventsModel>>(eventsQuery);
        }
    }
}
