using PhonebookApplication.Models;
using PhonebookApplication.Repositories;

namespace PhonebookApplication.Services
{
    public class InitialUserProvisioningService : IInitialUserProvisioningService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _configuration;

        public InitialUserProvisioningService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _configuration = configuration;
        }

        public async Task ProvisionAsync()
        {
            string? username = _configuration["InitialUser:Username"];

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException(
                    "Initial user configuration value 'InitialUser:Username' is missing.");
            }

            User? existingUser =
                await _userRepository.GetUserForLoginAsync(username);

            if (existingUser != null)
            {
                return;
            }

            string? password = _configuration["InitialUser:Password"];

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Initial user configuration value 'InitialUser:Password' is missing. Configure it through User Secrets or environment variables.");
            }

            var user = new User
            {
                Username = username
            };

            user.PasswordHash = _passwordService.HashPassword(user, password);

            await _userRepository.InsertUserIfNotExistsAsync(user);
        }
    }
}
