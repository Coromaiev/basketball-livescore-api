using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BasketBall_LiveScore.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository UserRepository;

        public UserService(IUserRepository userRepository)
        {
            this.UserRepository = userRepository;
        }

        public async Task<UserDto?> Create(UserCreateDto createDto)
        {
            if (createDto is null) return null;
            var existingUser = await UserRepository.GetByEmail(createDto.Email);
            if (existingUser is null)
            {
                var newUser = ConvertToEntity(createDto);
                await UserRepository.Create(newUser);
                return ConvertToDto(newUser);
            }

            return null;
        }

        public IEnumerable<UserDto?> GetAll()
        {
            var users = UserRepository.GetAll();
            if (users is null || !users.Any())
            {
                yield return null;
            }

            foreach (var user in users)
            {
                yield return ConvertToDto(user);
            }
        }

        public async Task<UserDto?> GetByEmailAndPassword(UserLoginDto loginDto)
        {
            if (loginDto is null) return null;
            var user = await UserRepository.GetByEmailAndPassword(loginDto.Email, loginDto.Password);
            if (user is null)
            {
                return null;
            }
            return ConvertToDto(user);
        }

        public async Task<UserDto?> GetById(Guid id)
        {
            var user = await UserRepository.GetById(id);
            if (user is null)
            {
                return null;
            }
            return ConvertToDto(user);
        }

        public IEnumerable<UserDto?> GetByRole(Role role)
        {
            var users = UserRepository.GetByRole(role);
            if (users is null || !users.Any())
            {
                yield return null;
            }
            
            foreach(var user in users)
            {
                yield return ConvertToDto(user);
            }
        }

        public async Task<UserDto?> Update(Guid id, UserUpdateDto updateDto)
        {
            if (updateDto is null) return null;
            var user = await UserRepository.GetById(id);
            if (user is null)
            {
                return null;
            }
            if (
                (!string.IsNullOrEmpty(updateDto.NewPassword) || !string.IsNullOrEmpty(updateDto.NewEmail)) 
                && (string.IsNullOrEmpty(updateDto.CurrentPassword) || !updateDto.CurrentPassword.Equals(user.Password))
               ) return null;
            var updatedUser = await UserRepository.Update(user, updateDto.NewEmail, updateDto.NewPassword, updateDto.NewUsername, updateDto.NewPermission);
            return ConvertToDto(updatedUser);
        }

        public async Task Delete(Guid id)
        {
            var userToDelete = await UserRepository.GetById(id);
            if (userToDelete is not null)
            {
                await UserRepository.Delete(userToDelete);
            }
        }

        private static User ConvertToEntity(UserCreateDto userDto)
        {
            var newUser = new User
            {
                Email = userDto.Email,
                Password = userDto.Password,
                Username = userDto.Username,
                Permission = userDto.Permission,
            };
            return newUser;
        }

        private static UserDto ConvertToDto(User user)
        {
            var userDto = new UserDto(user.Id, user.Username, user.Email, user.Permission);
            return userDto;
        }
    }
}
