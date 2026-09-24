using Microsoft.AspNetCore.Mvc;
using PhonebookApplication.Exceptions;
using PhonebookApplication.Models;
using PhonebookApplication.Repositories;

namespace PhonebookApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _repository;

        public ContactsController(IContactRepository repository)
        {
            _repository = repository;
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