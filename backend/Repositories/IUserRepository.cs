using PhonebookApplication.Models;

namespace PhonebookApplication.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserForLoginAsync(string username);

        Task<bool> InsertUserIfNotExistsAsync(User user);
    }
}
