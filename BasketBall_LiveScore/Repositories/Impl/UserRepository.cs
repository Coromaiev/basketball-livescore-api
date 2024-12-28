using BasketBall_LiveScore.Infrastructure;
using BasketBall_LiveScore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace BasketBall_LiveScore.Repositories.Impl
{
    public class UserRepository : IUserRepository
    {
        private readonly LiveScoreContext Context;

        public UserRepository(LiveScoreContext context)
        {
            Context = context;
        }

        public async Task<User> Create(User user)
        {
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            var users = Context.Users;
            foreach (var user in users)
            {
                yield return user;
            }
        }

        public async Task<User?> GetByEmail(string email)
        {
            var foundUser = await Context.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
            return foundUser;
        }

        public async Task<User?> GetByEmailAndPassword(string email, string password)
        {
            var foundUser = await Context.Users.FirstOrDefaultAsync(user => user.Email.Equals(email) && user.Password.Equals(password));
            return foundUser;
        }

        public async Task<User?> GetById(Guid id)
        {
            var foundUser = await Context.Users.FirstOrDefaultAsync(user => user.Id.Equals(id));
            return foundUser;
        }

        public IEnumerable<User> GetByRole(Role role)
        {
            var usersOfRole = Context.Users.Where(user => user.Permission.Equals(role));
            foreach (var user in usersOfRole)
            {
                yield return user;
            }
        }

        public async Task<User> Update(User user, string? newPassword, string? newEmail, string? newUsername, Role? newPermission)
        {
            if (!string.IsNullOrEmpty(newPassword)) user.Password = newPassword;
            if (!string.IsNullOrEmpty(newEmail)) user.Email = newEmail;
            if (!string.IsNullOrEmpty(newUsername)) user.Username = newUsername;
            if (newPermission is not null) user.Permission = (Role)newPermission;
            await Context.SaveChangesAsync();
            return user;
        }

        public async Task Delete(User user)
        {
            Context.Users.Remove(user);
            await Context.SaveChangesAsync();
        }
    }
}
