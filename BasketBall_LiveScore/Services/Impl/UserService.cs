using BasketBall_LiveScore.Exceptions;
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
            if (createDto is null) throw new BadRequestException("No creation data provided");
            if (string.IsNullOrWhiteSpace(createDto.Email) || string.IsNullOrWhiteSpace(createDto.Password) || string.IsNullOrWhiteSpace(createDto.Username))
                throw new BadRequestException("Invalid input data");
            var existingUser = await UserRepository.GetByEmail(createDto.Email);
            if (existingUser is null)
            {
                var newUser = ConvertToEntity(createDto);
                await UserRepository.Create(newUser);
                return ConvertToDto(newUser);
            }
            throw new ConflictException($"An account already exists for email {createDto.Email}");
        }

        public async IAsyncEnumerable<UserDto> GetAll()
        {
            var users = UserRepository.GetAll();
            if (users is null)
            {
                throw new NotFoundException("No users currently available");
            }

            await foreach (var user in users)
            {
                yield return ConvertToDto(user);
            }
        }

        public async Task<UserDto> GetByEmailAndPassword(UserLoginDto loginDto)
        {
            if (loginDto is null) throw new BadRequestException("No data provided");
            var user = await UserRepository.GetByEmailAndPassword(loginDto.Email, loginDto.Password) ?? throw new UnauthorizedException("Credentials provided are invalid");
            return ConvertToDto(user);
        }

        public async Task<UserDto> GetById(Guid id)
        {
            var user = await UserRepository.GetById(id) ?? throw new NotFoundException($"User with id {id} not found");
            return ConvertToDto(user);
        }

        public async IAsyncEnumerable<UserDto?> GetByRole(Role role)
        {
            var users = UserRepository.GetByRole(role);
            if (users is null)
            {
                throw new NotFoundException($"No user found for role {role}");
            }
            
            await foreach(var user in users)
            {
                yield return ConvertToDto(user);
            }
        }

        public async Task<UserDto?> Update(Guid id, UserUpdateDto updateDto)
        {
            if (updateDto is null) throw new BadRequestException("No update parameters provided");
            var user = await UserRepository.GetById(id) ?? throw new NotFoundException($"User with id {id} not found");
            if (
                (!string.IsNullOrEmpty(updateDto.NewPassword) || !string.IsNullOrEmpty(updateDto.NewEmail)) 
                && (string.IsNullOrEmpty(updateDto.CurrentPassword) || !updateDto.CurrentPassword.Equals(user.Password))
               ) throw new UnauthorizedException("Invalid Credentials for the requested update");
            var updatedUser = await UserRepository.Update(user, updateDto.NewEmail, updateDto.NewPassword, updateDto.NewUsername, updateDto.NewPermission);
            return ConvertToDto(updatedUser);
        }

        public async Task Delete(Guid id)
        {
            var userToDelete = await UserRepository.GetById(id) ?? throw new ConflictException($"User {id} does not exist or has already been deleted");
            await UserRepository.Delete(userToDelete);
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
