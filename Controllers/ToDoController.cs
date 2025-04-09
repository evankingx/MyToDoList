using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyToDoList.Models.DTOs;
using MyToDoList.Models.Entities;
using MyToDoList.Data;

namespace TodoApiMvc.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ToDoDbContext _context;

        public TodoController(ToDoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/tasks
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _context.Tasks
            .AsNoTracking()
                .OrderBy(t => t.Id)
                .Select(t => new ToDoItemsDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted
                })
                .ToListAsync();
            return Ok(tasks);
        }

        // GET: api/tasks/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound(new { error = $"Task with ID {id} not found" });

            return Ok(new ToDoItemsDto
            {
                Id = task.Id,
                Title = task.Title,
                IsCompleted = task.IsCompleted
            });
        }

        // POST: api/tasks
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTask([FromBody] ToDoItemsCreateDto request)
        {
            try
            {
                var task = new ToDoItems(request.Title, request.IsCompleted);
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                var response = new ToDoItemsDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    IsCompleted = task.IsCompleted
                };

                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT: api/tasks/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] ToDoItemsCreateDto request)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                    return NotFound(new { error = $"Task with ID {id} not found" });

                task.Update(request.Title, request.IsCompleted);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/tasks/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound(new { error = $"Task with ID {id} not found" });

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}