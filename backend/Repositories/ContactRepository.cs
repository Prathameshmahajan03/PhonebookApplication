using Microsoft.Data.SqlClient;
using PhonebookApplication.Exceptions;
using PhonebookApplication.Models;

namespace PhonebookApplication.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly string _connectionString;

        public ContactRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<ContactExportRecord>> GetContactsForExportAsync()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(
                "sp_GetContactsForExport",
                connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            var contacts = new List<ContactExportRecord>();

            while (await reader.ReadAsync())
            {
                contacts.Add(new ContactExportRecord
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Email")),
                    Address = reader.IsDBNull(reader.GetOrdinal("Address"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Address"))
                });
            }

            return contacts;
        }

        public async Task<Contact?> GetContactByIdAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(
                "sp_GetContactById",
                connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Contact
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Email")),
                    Address = reader.IsDBNull(reader.GetOrdinal("Address"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Address")),
                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                };
            }

            return null;
        }

        public async Task<int> InsertContactAsync(Contact contact)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(
                "sp_InsertContact",
                connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", contact.Name);
            command.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
            command.Parameters.AddWithValue("@Email",
                (object?)contact.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address",
                (object?)contact.Address ?? DBNull.Value);

            await connection.OpenAsync();

            try
            {
                object? result = await command.ExecuteScalarAsync();

                return Convert.ToInt32(result);
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                throw new DuplicatePhoneException(
                    "Phone number already exists.");
            }


        }


        public async Task<bool> UpdateContactAsync(int id, Contact contact)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(
                "sp_UpdateContact",
                connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", contact.Name);
            command.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
            command.Parameters.AddWithValue("@Email",
                (object?)contact.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address",
                (object?)contact.Address ?? DBNull.Value);

            await connection.OpenAsync();

            try
            {
                int rowsAffected = Convert.ToInt32(
                    await command.ExecuteScalarAsync());

                return rowsAffected > 0;
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                throw new DuplicatePhoneException(
                    "Phone number already exists.");
            }
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(
                "sp_DeleteContact",
                connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();

            int rowsAffected = Convert.ToInt32( await command.ExecuteScalarAsync());

            return rowsAffected > 0;
        }

        public async Task<PagedResult<Contact>> GetContactsPagedAsync(
                                                                       int pageNumber, int pageSize, string? searchTerm)
        {

            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand command = new SqlCommand(
                "sp_GetContactsPaged",
                connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@PageNumber", pageNumber);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@SearchTerm",
                (object?)searchTerm ?? DBNull.Value);

            await connection.OpenAsync();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            List<Contact> contacts = new();

            while (await reader.ReadAsync())
            {
                contacts.Add(new Contact
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Email")),
                    Address = reader.IsDBNull(reader.GetOrdinal("Address"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Address")),
                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }

            await reader.NextResultAsync();

            int totalCount = 0;

            if (await reader.ReadAsync())
            {
                totalCount = reader.GetInt32(
                    reader.GetOrdinal("TotalCount"));
            }

            return new PagedResult<Contact>
            {
                Items = contacts,
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

        }
    }
}