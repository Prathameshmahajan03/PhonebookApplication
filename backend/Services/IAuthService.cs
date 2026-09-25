using PhonebookApplication.Models;

namespace PhonebookApplication.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
