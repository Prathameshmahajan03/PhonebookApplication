using System.Data;
using Microsoft.Data.SqlClient;
using PhonebookApplication.Models;

namespace PhonebookApplication.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<User?> GetUserForLoginAsync(string username)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(
                "dbo.sp_GetUserForLogin",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                new SqlParameter("@Username", SqlDbType.NVarChar, 100)
                {
                    Value = username
                });

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                };
            }

            return null;
        }

        public async Task<bool> InsertUserIfNotExistsAsync(User user)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(
                "dbo.sp_InsertUserIfNotExists",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(
                new SqlParameter("@Username", SqlDbType.NVarChar, 100)
                {
                    Value = user.Username
                });

            command.Parameters.Add(
                new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 512)
                {
                    Value = user.PasswordHash
                });

            await connection.OpenAsync();

            try
            {
                object? result = await command.ExecuteScalarAsync();

                return Convert.ToInt32(result) > 0;
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return false;
            }
        }
    }
}
