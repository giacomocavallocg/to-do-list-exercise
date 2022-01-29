using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoDomain.Sql.Context;
using ToDoDomain.Sql.Models;
using ToDoWebApp.Api.Common;
using ToDoWebApp.Dtos;

namespace ToDoWebApp.Api
{
    [ApiController]
    [DefaultRoute]
    public class ToDoEntryController : BaseController
    {

        private readonly ToDoEntryRepository toDoEntryRepository;
        private readonly ToDoListRepository toDoListRepository;


        public ToDoEntryController(ToDoDbContext dbContext) : base(dbContext)
        {
            toDoEntryRepository = new ToDoEntryRepository(dbContext);
            toDoListRepository = new ToDoListRepository(dbContext);

        }

        /// <summary>
        /// Get the list of entries contained in a toDo list.
        /// </summary>
        /// <param name="listId">toDo list id</param>
        /// <returns>A list of entiries</returns>
        [ProducesResponseType(typeof(List<ToDoEntryDto>), StatusCodes.Status200OK)]
        [HttpGet("{listId}/entires")]
        public async Task<ActionResult<List<ToDoEntryDto>>> GetByListId([FromRoute] string listId)
        {
            var entries = await toDoEntryRepository.FindAll().Where(e => e.ToDoListId == listId).ToListAsync();

            return entries.ConvertAll(e => new ToDoEntryDto(e));
        }


        /// <summary>
        /// Get single entry.
        /// </summary>
        /// <param name="id">entry id</param>
        /// <returns>Entry</returns>
        [ProducesResponseType(typeof(ToDoEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("entry/{id}")]
        public async Task<ActionResult<ToDoEntryDto>> Get([FromRoute] string id)
        {
            var entry = await toDoEntryRepository.FindAll().FirstOrDefaultAsync(l => l.Id == id);

            if (entry is null)
                return new NotFoundObjectResult($"Entry with id {id} not found");

            return new ToDoEntryDto(entry);
        }

        /// <summary>
        /// Create new Entry
        /// </summary>
        /// <remarks>TodoList is requeired. Content and Expiration Date are oprional, other attruibute are ignored</remarks>
        /// <param name="entryDto">Entry</param>
        /// <returns>The new created Entry</returns>
        [ProducesResponseType(typeof(ToDoEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("entry")]
        public async Task<ActionResult<ToDoEntryDto>> Save([FromBody] ToDoEntryDto entryDto)
        {
            var toDoEntry = new ToDoEntry()
            {
                Content = entryDto.Content,
                ToDoListId = entryDto.ToDoListId,
                ExpirationDate = entryDto.ExpirationDate,
            };

            var listExists = await toDoListRepository.FindAll().AnyAsync(l => l.Id == entryDto.ToDoListId);
            if (!listExists)
                return new NotFoundObjectResult("Entry list not found");

            toDoEntry = await toDoEntryRepository.Save(toDoEntry);
            await ToDoDbContext.SaveChangesAsync();

            return new ToDoEntryDto(toDoEntry);
        }


        /// <summary>
        /// Update Entry
        /// </summary>
        /// <remarks>Only Content and ExpirationDate can be updated</remarks>
        /// <param name="id">Entry id to be updated</param>
        /// <param name="entryDto">Entry updated content</param>
        /// <returns>The updatet entry</returns>
        [ProducesResponseType(typeof(ToDoEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("entry/{id}")]
        public async Task<ActionResult<ToDoEntryDto>> Update([FromRoute] string id, [FromBody, BindRequired] ToDoEntryDto entryDto)
        {
            var toDoEntry = await toDoEntryRepository.FindAll().FirstOrDefaultAsync(l => l.Id == id);

            if (toDoEntry is null)
                return new NotFoundObjectResult($"Entry with id {id} not found");

            toDoEntry.Content = entryDto.Content;
            toDoEntry.ExpirationDate = entryDto.ExpirationDate;

            toDoEntry = await toDoEntryRepository.Save(toDoEntry);
            await ToDoDbContext.SaveChangesAsync();

            return new ToDoEntryDto(toDoEntry);
        }


        /// <summary>
        /// Done entry
        /// </summary>
        /// <remarks>Check entry as done</remarks>
        /// <param name="id">Entry id</param>
        /// <returns>The updated entry</returns>
        [ProducesResponseType(typeof(ToDoEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("entry/{id}/done")]
        public async Task<ActionResult<ToDoEntryDto>> SetDone([FromRoute] string id)
        {
            var entry = await SetStatus(id, true);
            if(entry is null)
                return new NotFoundObjectResult($"Entry with id {id} not found");

            return entry;
        }

        /// <summary>
        /// Undone entry
        /// </summary>
        /// <remarks>Check entry as undone</remarks>
        /// <param name="id">Entry id</param>
        /// <returns>The updated entry</returns>
        [ProducesResponseType(typeof(ToDoEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("entry/{id}/undone")]
        public async Task<ActionResult<ToDoEntryDto>> SetUndone([FromRoute] string id)
        {
            var entry = await SetStatus(id, false);
            if (entry is null)
                return new NotFoundObjectResult($"Entry with id {id} not found");

            return entry;
        }


        /// <summary>
        /// Delete entry
        /// </summary>
        /// <remarks>Delete an existing entry</remarks>
        /// <param name="id">Entry id</param>
        [ProducesResponseType(typeof(ToDoEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("entry/{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            var toDoEntry = await toDoEntryRepository.FindAll().FirstOrDefaultAsync(l => l.Id == id);

            if (toDoEntry is null)
                return new NotFoundObjectResult($"List with id {id} not found");

            toDoEntryRepository.Delete(toDoEntry);
            await ToDoDbContext.SaveChangesAsync();

            return new OkResult();
        }

        private async Task<ToDoEntryDto> SetStatus(string id, bool isDone)
        {
            var toDoEntry = await toDoEntryRepository.FindAll().FirstOrDefaultAsync(l => l.Id == id);

            if (toDoEntry is null)
                return null;

            toDoEntry.IsDone = isDone;

            toDoEntry = await toDoEntryRepository.Save(toDoEntry);
            await ToDoDbContext.SaveChangesAsync();

            return new ToDoEntryDto(toDoEntry);
        }

    }
}
