using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhonebookApplication.Exceptions;
using PhonebookApplication.Models;
using PhonebookApplication.Repositories;
using PhonebookApplication.Services;

namespace PhonebookApplication.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _repository;
        private readonly IContactExportService _exportService;

        public ContactsController(
            IContactRepository repository,
            IContactExportService exportService)
        {
            _repository = repository;
            _exportService = exportService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<Contact>>> GetContacts(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null)
        {

            if (pageNumber < 1)
            {
                return BadRequest("Page number must be greater than or equal to 1.");
            }

            if (pageSize < 1)
            {
                return BadRequest("Page size must be greater than or equal to 1.");
            }

            var result = await _repository.GetContactsPagedAsync(
                pageNumber,
                pageSize,
                searchTerm
            );

            return Ok(result);
        }

        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportContactsCsv()
        {
            List<ContactExportRecord> contacts =
                await _repository.GetContactsForExportAsync();

            byte[] file = _exportService.CreateCsv(contacts);

            return File(file, "text/csv; charset=utf-8", "contacts.csv");
        }

        [HttpGet("export/json")]
        public async Task<IActionResult> ExportContactsJson()
        {
            List<ContactExportRecord> contacts =
                await _repository.GetContactsForExportAsync();

            byte[] file = _exportService.CreateJson(contacts);

            return File(file, "application/json; charset=utf-8", "contacts.json");
        }

        [HttpPost]
        public async Task<ActionResult<Contact>> CreateContact(Contact contact)
        {
            try
            {
                int newId = await _repository.InsertContactAsync(contact);

                var createdContact = await _repository.GetContactByIdAsync(newId);

                return CreatedAtAction(
                    nameof(GetContactById),
                    new { id = newId },
                    createdContact);
            }
            catch (DuplicatePhoneException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContactById(int id)
        {
            var contact = await _repository.GetContactByIdAsync(id);

            if (contact == null)
            {
                return NotFound();
            }
                
            return Ok(contact);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, Contact contact)
        {
            try
            {
                var updated = await _repository.UpdateContactAsync(id, contact);

                if (!updated)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (DuplicatePhoneException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var deleted = await _repository.DeleteContactAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}