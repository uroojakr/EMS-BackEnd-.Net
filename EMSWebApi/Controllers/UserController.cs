using AutoMapper;
using EMS.Business.Interfaces;
using EMS.Business.Models;
using EMS.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace EMSWebApi.Controllers
{

    [Route("api/[controller]")]
    //[ApiController]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UserController(IUserService userService, IMapper mapper, ILogger<UserController> logger)
        {
            _userService = userService!;
            _mapper = mapper;
            _logger = logger;
        }



        // GET : api/Users/userType
        [HttpGet("ByUserType/{userType}")]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUserByUserType(UserType userType)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve users by user type: {UserType} ", userType);
                var users = await _userService.GetUserByUserType(userType);

                if (users == null)
                {
                    _logger.LogInformation("No users found with user type : {UserType}", userType);
                    return NotFound(new { message = " No users found with specified userType" });
                }


                var userModels = _mapper.Map<IEnumerable<UserModel>>(users);
                _logger.LogInformation("Successfully ran GetByUserType");
                return Ok(new { message = "Successfully retrieved", user = userModels });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving users by user type: {Error}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while retrieving users by user type", error = ex.Message });
            }
        }



        [HttpPut("ChangePassword/{userId}")]
        [Authorize(Roles = "Administrator, Organizer")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangePasswordModel changePasswordModel)
        {
            try
            {
                var user = await _userService.GetByIdAsync(userId);

                if (user != null && user.Password == changePasswordModel.OldPassword)
                {
                    user.Password = changePasswordModel.NewPassword;
                    await _userService.UpdateAsync(userId, user);
                    return Ok(new { message = "Successfully Updated Password" });
                }
                return BadRequest(new { message = "Invalid password or user" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while changing password for user ID: {userID}, Error: {Error}", userId, ex.Message);
                return StatusCode(500, new { message = " An erro occurred while changing the password", error = ex.Message });
            }
        }

        //GET : api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<ActionResult<object>> GetUser(int id)
        {
            _logger.LogInformation("Attempting to retrieve user with ID: {UserID}", id);

            var user = await _userService.GetUser(id);

            if (user == null)
            {
                _logger.LogError("User Not found with specified ID: {UserID}", id);
                return NotFound(new { message = "User not found with specified ID" });
            }

            _logger.LogInformation("Successfully ran GetUser");
            return Ok(user);
        }

        //POST: api/Users
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateUser([FromBody] UserModel userModel)
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

            var user = _mapper.Map<User>(userModel);
            await _userService.CreateAsync(userModel);
            var createdUserModel = _mapper.Map<UserModel>(user);
            return Ok(new { message = "User Created" });
        }


        [Authorize(Roles = "Administrator")]
        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserModel updatedUserModel)
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
                if (updatedUserModel == null)
                {
                    return BadRequest(new { message = "Invalid user data" });
                }

                var existingUser = await _userService.GetByIdAsync(id);

                existingUser.UserName = updatedUserModel.UserName;
                existingUser.Email = updatedUserModel.Email;
                existingUser.Password = updatedUserModel.Password;
                existingUser.UserType = updatedUserModel.UserType;

                await _userService.UpdateAsync(id, existingUser);

                return Ok(new { message = " User Updated Successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred with ID: {UserID}, Error: {Error}", id, ex.Message);
                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }
        }


        //DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "{id} Not found!", id });
            }
            await _userService.DeleteAsync(id);
            return Ok(new { message = "Deleted Successfully" });
        }

        //GET: api/Users/GetAllUsers
        [HttpGet()]
        [Authorize(Roles = "Administrator,Organizer")]
        public async Task<ActionResult<object>> GetAll()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all Users");
                var users = await _userService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError("An  error occured ", ex.Message);
                return NotFound(new { message = "Events not found" });
            }
        }


    }
}