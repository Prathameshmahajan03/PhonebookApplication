using PhonebookApplication.Models;

namespace PhonebookApplication.Repositories
{
    public interface IContactRepository
    {
        Task<PagedResult<Contact>> GetContactsPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm);

        Task<List<ContactExportRecord>> GetContactsForExportAsync();

        Task<Contact?> GetContactByIdAsync(int id);

        Task<int> InsertContactAsync(Contact contact);

        Task<bool> UpdateContactAsync(int id, Contact contact);

        Task<bool> DeleteContactAsync(int id);
    }
}