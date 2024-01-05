using AutoMapper;
using EMS.Business.Interfaces;
using EMS.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EMSWebApi.Controllers
{
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventsService _eventsService;
        private readonly IMapper _mapper;
        private readonly ILogger<EventController> _logger;

        public EventController(IEventsService eventsService, IMapper mapper, ILogger<EventController> logger)
        {
            _eventsService = eventsService;
            _mapper = mapper;
            _logger = logger;
        }

        //Get : api/Event/5
        [HttpGet]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<ActionResult<EventsModel>> GetAll()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all Events");
                var eventModel = await _eventsService.GetAllAsync();

                if (eventModel == null)
                {
                    _logger.LogError("Error while retrieving data");
                    return NotFound(new { message = "Error while retrieving all data" });
                }
                return Ok(new { message = "Retrieved Successfully", events = eventModel });
            }
            catch (Exception ex)
            {
                _logger.LogError("An  error occured ", ex.Message);
                return NotFound(new { message = "Events not found" });
            }
        }

        [HttpGet("ByLocation")]
        public async Task<ActionResult<IEnumerable<EventsModel>>> GetEventsByLocation([Required] string location)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve events by location: {Location}", location);
                var events = await _eventsService.GetEventsByLocation(location);

                if (events == null || !events.Any())
                {
                    _logger.LogInformation("No events found at the specified location: {Location}", location);
                    return NotFound(new { message = "No Events found at location specified: {Location}", location });
                }
                var eventModels = _mapper.Map<IEnumerable<EventsModel>>(events);

                _logger.LogInformation("Successfully ran GetEventsByLocation");
                return Ok(new { message = "Sucessfully retrieved events by location ", events = eventModels });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving events by location: {Error} ", ex.Message);
                return StatusCode(500, new { message = " An error occurred while retrieving events by location: {Error}", ex.Message });
            }
        }

        // GET: api/Event
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<ActionResult<EventsModel>> GetEvent(int id)
        {
            _logger.LogInformation("Attempting to retrieve events by ID");
            var eventModel = await _eventsService.GetByIdAsync(id);

            if (eventModel == null)
            {
                _logger.LogError("Events not found with specified ID: {EventID}", id);
                return NotFound(new { message = "Event not found with specified ID" });
            }

            var eventEntity = _mapper.Map<EventsModel>(eventModel);
            _logger.LogInformation("Successfully ran GetEvent");
            return Ok(new { message = "Successfully Retrieved", events = eventEntity });
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<bool>> CreateEvent([FromBody] EventsModel eventsModel)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values
                    .SelectMany(e => e.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                _logger.LogError("Validation error: {ValidationError}", error);

                return BadRequest(new { error = error });
            }

            try
            {
                var result = await _eventsService.CreateAsync(eventsModel);

                if (result)
                {
                    _logger.LogInformation("New Event Created Successfully");
                    return Ok(new { message = "New Event Created Successfully" });
                }
                else
                {
                    _logger.LogError("Error in Creating new Event");
                    return BadRequest(new { message = "Error in Creating new Event" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the event: {ErrorMessage}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while creating the event" });
            }
        }


        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventsModel updatedEventModel)
        {
            try
            {
                if (updatedEventModel == null)
                {
                    return BadRequest(new { message = "Invalid Event" });
                }

                var existingEvent = await _eventsService.GetByIdAsync(id);
                if (existingEvent == null)
                {
                    return NotFound(new { message = "Event not found with specified id" });
                }
                existingEvent.Title = updatedEventModel.Title;
                existingEvent.Description = updatedEventModel.Description;
                existingEvent.Location = updatedEventModel.Location;
                existingEvent.Date = updatedEventModel.Date;
                existingEvent.OrganizerId = updatedEventModel.OrganizerId;

                await _eventsService.UpdateAsync(id, existingEvent);
                return Ok(new { message = "Event Updated Successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred with ID: {EventID}, {errorMessage}", id, ex.Message);
                return StatusCode(500, new { message = $"Error Occurred: {ex.Message}" });
            }
        }

        //DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var events = await _eventsService.GetByIdAsync(id);
            if (events == null)
            {
                return NotFound(new { message = "{userID} Not found!", id });
            }

            await _eventsService.DeleteAsync(id);
            return Ok(new { message = "Deleted Successfully" });
        }

    }
}
