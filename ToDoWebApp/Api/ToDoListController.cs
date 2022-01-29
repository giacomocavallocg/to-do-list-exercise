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
    public class ToDoListController : BaseController
    {

        private readonly ToDoListRepository toDoListRepository;
        public ToDoListController(ToDoDbContext dbContext) : base(dbContext)
        {
            toDoListRepository = new ToDoListRepository(dbContext);

        }

        /// <summary>
        /// Get all toDo list
        /// </summary>
        /// <returns>List of toDo list</returns>
        [ProducesResponseType(typeof(ToDoListDto), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<List<ToDoListDto>>> GetAll()
        {
            var lists = await toDoListRepository.FindAll().ToListAsync();
            return lists.ConvertAll(l => new ToDoListDto(l));
        }


        /// <summary>
        /// Get todo list
        /// </summary>
        /// <param name="id">id of the toDo list</param>
        /// <returns>toDo list</returns>
        [ProducesResponseType(typeof(ToDoListDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoListDto>> Get([FromRoute] string id)
        {
            var list = await toDoListRepository.FindAll().FirstOrDefaultAsync(l => l.Id == id);

            if (list is null)
                return new NotFoundObjectResult($"List with id {id} not found");

            return new ToDoListDto(list);
        }

        /// <summary>
        /// Create new toDo list
        /// </summary>
        /// <remarks>Name attribute is optional. Other params is ignored</remarks>
        /// <param name="listDto">toDo list</param>
        /// <returns>New created toDo list</returns>
        [ProducesResponseType(typeof(ToDoListDto), StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<ActionResult<ToDoListDto>> Save([FromBody] ToDoListDto listDto)
        {
            var toDoList = new ToDoList()
            {
                Name = listDto.Name,
            };

            toDoList = await toDoListRepository.Save(toDoList);
            await ToDoDbContext.SaveChangesAsync();

            return new ToDoListDto(toDoList);
        }

        /// <summary>
        /// Update toDo list
        /// </summary>
        /// <remarks>Only name can be updated.</remarks>
        /// <param name="listDto">toDo list</param>
        /// <returns>Updated toDo list</returns>
        [ProducesResponseType(typeof(ToDoListDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}")]
        public async Task<ActionResult<ToDoListDto>> Update([FromRoute] string id, [FromBody, BindRequired] ToDoListDto listDto)
        {
            var toDoList = await toDoListRepository.FindAll().FirstOrDefaultAsync(l => l.Id == id);

            if (toDoList is null)
                return new NotFoundObjectResult($"List with id {id} not found");

            toDoList.Name = listDto.Name;

            toDoList = await toDoListRepository.Save(toDoList);
            await ToDoDbContext.SaveChangesAsync();

            return new ToDoListDto(toDoList);
        }

        /// <summary>
        /// Delete toDo list
        /// </summary>
        /// <remarks>All the connected entries are deleted too</remarks>
        /// <param name="id">Id of toDo list</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            var toDoList = await toDoListRepository.FindAll().FirstOrDefaultAsync(l => l.Id == id);

            if (toDoList is null)
                return new NotFoundObjectResult($"List with id {id} not found");
                
            using(var transaction = await ToDoDbContext.Database.BeginTransactionAsync())
            {

                try
                {

                    await toDoListRepository.Delete(toDoList);
                    await ToDoDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch(Exception e)
                {
                    await transaction.RollbackAsync();
                    throw e;
                }
            }

            return new OkResult();
        }

    }
}
