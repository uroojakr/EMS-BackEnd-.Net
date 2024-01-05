using AutoMapper;
using EMS.Business.Interfaces;
using EMS.Business.Models;
using EMS.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EMSWebApi.Controllers
{
    [Route("api/[controller]")]
    public class VendorController : Controller
    {
        private readonly IVendorService _vendorService;
        private readonly IMapper _mapper;
        private readonly ILogger<VendorController> _logger;

        public VendorController(IVendorService vendorService, IMapper mapper, ILogger<VendorController> logger)
        {
            _vendorService = vendorService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Vendor/Count
        [HttpGet("Count")]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<IActionResult> GetVendorCountAsync()
        {
            try
            {
                var count = await _vendorService.GetVendorCountAsync();
                return Ok(new { message = "Vendor count retrieved successfully", count });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while retrieving vendor count: {Error}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while retrieving vendor count", error = ex.Message });
            }
        }

        //GET : api/Vendor/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<ActionResult<Vendor>> GetVendor(int id)
        {
            _logger.LogInformation("Attempting to retrieve Vendor with ID: {VendorId}", id);

            var vendor = await _vendorService.GetByIdAsync(id);

            if (vendor == null)
            {
                _logger.LogError("Vendor Not found with specified ID: {VendorId}", id);
                return NotFound(new { message = "Vendor not found with specified ID" });
            }

            var VendorEntity = _mapper.Map<Vendor>(vendor);
            _logger.LogInformation("Successfully ran GetVendor");
            return Ok(new { message = "Successfully Retrieved", vendor = VendorEntity });
        }



        // POST: api/Vendor
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateVendor([FromBody] VendorModel vendorModel)
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

            var vendor = _mapper.Map<Vendor>(vendorModel);
            await _vendorService.CreateAsync(vendorModel);
            var createdVendorModel = _mapper.Map<VendorModel>(vendor);
            return Ok(new { message = "Vendor Created", vendor = createdVendorModel });
        }

        // PUT: api/Vendor/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateVendor(int id, [FromBody] VendorModel updatedVendorModel)
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

                var existingVendor = await _vendorService.GetByIdAsync(id);

                if (existingVendor == null)
                {
                    return NotFound(new { message = "Vendor not found with specified ID", id });
                }

                existingVendor.Name = updatedVendorModel.Name;
                existingVendor.Description = updatedVendorModel.Description;
                existingVendor.ContactInformation = updatedVendorModel.ContactInformation;

                await _vendorService.UpdateAsync(id, existingVendor);

                return Ok(new { message = "Vendor Updated Successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred with ID: {VendorId}, Error: {Error}", id, ex.Message);
                return StatusCode(500, new { message = "An error occurred while updating the Vendor", error = ex.Message });
            }
        }

        // DELETE: api/Vendor/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var vendor = await _vendorService.GetByIdAsync(id);
            if (vendor == null)
            {
                return NotFound(new { message = $"{id} Not found!" });
            }

            await _vendorService.DeleteAsync(id);

            return Ok(new { message = "Deleted Successfully" });
        }
    }
}
