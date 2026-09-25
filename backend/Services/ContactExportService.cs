using System.Globalization;
using System.Text;
using System.Text.Json;
using PhonebookApplication.Models;

namespace PhonebookApplication.Services
{
    public class ContactExportService : IContactExportService
    {
        private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web);

        public byte[] CreateCsv(IReadOnlyCollection<ContactExportRecord> contacts)
        {
            var csv = new StringBuilder();

            csv.AppendLine("Id,Name,PhoneNumber,Email,Address");

            foreach (ContactExportRecord contact in contacts)
            {
                csv.Append(EscapeCsv(contact.Id.ToString(CultureInfo.InvariantCulture)));
                csv.Append(',');
                csv.Append(EscapeCsv(contact.Name));
                csv.Append(',');
                csv.Append(EscapeCsv(contact.PhoneNumber));
                csv.Append(',');
                csv.Append(EscapeCsv(contact.Email));
                csv.Append(',');
                csv.AppendLine(EscapeCsv(contact.Address));
            }

            return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
                .GetBytes(csv.ToString());
        }

        public byte[] CreateJson(IReadOnlyCollection<ContactExportRecord> contacts)
        {
            return JsonSerializer.SerializeToUtf8Bytes(contacts, JsonOptions);
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            bool requiresQuotes = value.Contains(',')
                || value.Contains('"')
                || value.Contains('\r')
                || value.Contains('\n');

            if (!requiresQuotes)
            {
                return value;
            }

            return $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
        }
    }
}
