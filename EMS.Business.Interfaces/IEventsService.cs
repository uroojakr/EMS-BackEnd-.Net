using EMS.Business.Models;
using EMS.Data.Models;

namespace EMS.Business.Interfaces
{
    public interface IEventsService : IGenericCrudService<Events, EventsModel>
    {
        Task<IEnumerable<EventsModel>> GetEventsByLocation(string location);

    }
}
