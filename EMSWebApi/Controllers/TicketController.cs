using AutoMapper;
using EMS.Business.Models;
using EMS.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EMSWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketController> _logger;

        public TicketController(ITicketService ticketService, IMapper mapper, ILogger<TicketController> logger)
        {
            _ticketService = ticketService;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<IActionResult> GetUserTickets(int userId)
        {
            var tickets = await _ticketService.GetTicketsByUserId(userId);
            if (tickets == null || !tickets.Any())
            {
                return NotFound(new { message = $"Not found by userId: {userId}" });
            }
            var ticketModels = _mapper.Map<IEnumerable<TicketModel>>(tickets);
            _logger.LogInformation("Successfully ran GetUserTickets");
            return Ok(new { message = "Successfully Retrieved", tickets = ticketModels });
        }

        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<IActionResult> GetEventTickets(int eventId)
        {
            var tickets = await _ticketService.GetTicketsByEventId(eventId);
            if (tickets == null || !tickets.Any())
            {
                return NotFound(new { message = $"Not found by eventId: {eventId}" });
            }

            var ticketModels = _mapper.Map<IEnumerable<TicketModel>>(tickets);

            _logger.LogInformation("Successfully ran GetEventTickets");
            return Ok(new { message = "Successfully Retrieved", tickets = ticketModels });
        }


        //POST: api/Tickets
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateTicket([FromBody] TicketModel ticketModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value!.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                return BadRequest(new { message = "Validation Failed", errors });
            }
            var ticket = _mapper.Map<Ticket>(ticketModel);
            await _ticketService.CreateAsync(ticketModel);
            var createdUserModel = _mapper.Map<TicketModel>(ticket);
            return Ok(new { message = "Ticket Created" });
        }

        //PUT: api/Ticket
        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] TicketModel updatedTicketModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(e => e.Value!.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(new { message = "Validation Failed", errors });
                }
                if (updatedTicketModel == null)
                {
                    return BadRequest(new { message = "Invalid ticket data" });
                }
                var existingTicket = await _ticketService.GetByIdAsync(id);

                if (existingTicket == null)
                {
                    return NotFound(new { message = $"Ticket with ID {id} not found" });
                }

                existingTicket.UserId = updatedTicketModel.UserId;
                existingTicket.EventId = updatedTicketModel.EventId;

                await _ticketService.UpdateAsync(id, existingTicket);
                return Ok(new { message = "Ticket Updated Successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred with ID: {id}, Error: {Error}", id, ex.Message);
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception details available";
                return StatusCode(500, new { message = "An error occurred while updating the ticket", error = ex.Message, innerError = innerExceptionMessage });
            }
        }

        //DELETE: api/Ticket/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound(new { message = "{id} Not found!", id });
            }
            await _ticketService.DeleteAsync(id);
            return Ok(new { message = "Deleted Successfully" });
        }

        //GET: api/Ticket/
        [HttpGet]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllAsync();
            if (tickets == null || !tickets.Any())
            {
                return NotFound(new { message = "No tickets found" });
            }
            var ticketModels = _mapper.Map<IEnumerable<TicketModel>>(tickets);

            _logger.LogInformation("Successfully retrieved all tickets");
            return Ok(new { message = "Successfully Retrieved", tickets = ticketModels });
        }

        //GET: api/Ticket/{id}
        [HttpGet("{id}", Name = "GetTicketById")]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound(new { message = $"Ticket with ID {id} not found" });
            }

            var ticketModel = _mapper.Map<TicketModel>(ticket);

            _logger.LogInformation("Successfully retrieved ticket by ID");
            return Ok(new { message = "Successfully Retrieved", ticket = ticketModel });
        }

    }
}
