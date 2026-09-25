using PhonebookApplication.Models;

namespace PhonebookApplication.Services
{
    public interface IContactExportService
    {
        byte[] CreateCsv(IReadOnlyCollection<ContactExportRecord> contacts);

        byte[] CreateJson(IReadOnlyCollection<ContactExportRecord> contacts);
    }
}
