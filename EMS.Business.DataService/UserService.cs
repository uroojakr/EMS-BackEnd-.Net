using AutoMapper;
using EMS.Business.Interfaces;
using EMS.Business.Models;
using EMS.Data.Interfaces;
using EMS.Data.Models;

namespace EMS.Business.DataService
{
    public class UserService : GenericCrudService<User, UserModel>, IUserService
    {
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<UserModel> AuthenticateUser(string username, string password)
        {
            var users = await _repository.GetAll();
            var user = users.FirstOrDefault(u => u.UserName == username && u.Password == password);

            if (user == null)
            {
                return null!;
            } 

            return _mapper.Map<UserModel>(user);
        }

        public async Task<IEnumerable<UserModel>> GetUserByUserType(UserType userType)
        {
            var users = await _repository.GetAll();
            var usersQuery = users.Where(u => u.UserType == userType).ToList();
            return _mapper.Map<IEnumerable<UserModel>>(usersQuery);
        }

        public async Task<User> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var users = await _repository.GetAll();
            var user = users.SingleOrDefault(u => u.Id == userId);

            if (user != null && user.Password == oldPassword)
            {
                user.Password = newPassword;

                _repository.Update(user);
                await _unitOfWork.SaveChangesAsync();
                return user;
            }
            return null!;
        }
        // Get All Users
        public async Task<IEnumerable<object>> GetAllUsers()
        {
            var users = await _repository.GetAll();

            return users;
        }
        // Get User By id
        public async Task<object> GetUser(int id)
        {
            var users = await _repository.GetAll();
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user is null)
            {
                Console.Write("User Not Found");
            }
            return user!;
        }
    }
}
