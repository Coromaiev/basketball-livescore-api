using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Services
{
    public interface IUserService
    {
        public Task<UserDto> GetById(Guid id);
        public Task<UserDto> GetByEmailAndPassword(UserLoginDto loginDto);
        public Task<UserDto?> Create(UserCreateDto createDto);
        public Task<UserDto> Update(Guid id, UserUpdateDto updateDto);
        public Task Delete(Guid id);
        public IAsyncEnumerable<UserDto?> GetAll();
        public IAsyncEnumerable<UserDto?> GetByRole(Role role);
    }
}
