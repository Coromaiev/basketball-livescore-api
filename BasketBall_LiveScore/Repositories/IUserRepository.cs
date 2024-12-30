using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Repositories
{
    public interface IUserRepository
    {
        public Task<User?> GetById(Guid id);
        public IAsyncEnumerable<User> GetAll();
        public IAsyncEnumerable<User> GetByRole(Role role);
        public Task<User?> GetByEmail(string email);
        public Task<User?> GetByEmailAndPassword(string email, string password);
        public Task<User> Create(User user);
        public Task<User> Update(User user, string? newEmail, string? newPassword, string? newUsername, Role? newPermission);
        public Task Delete(User user);
    }
}
