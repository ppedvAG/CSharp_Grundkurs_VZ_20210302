using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab16_API.Data;
using Lab16_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab16_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItem()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            TodoItem item = await _context.TodoItems.FindAsync(id);

            if (item == null)
                return NotFound();

            return item;
        }

        // GET: api/TodoItems/5-10
        [HttpGet("{id}-{id2}")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItem(long id, long id2)
        {
            return await _context.TodoItems.Where(todoitem => todoitem.Id >= id && todoitem.Id <= id2).ToListAsync(); 
        }

        // POST: api/TodoItems/ + Body=item
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem item)
        {
            try
            {
                _context.TodoItems.Add(item);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetTodoItem", new { id = item.Id }, item);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        // POST: api/TodoItems/BulkPost + Body=DTOItem (Data Transfer Object)
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<TodoItem>> BulkPost(TodoItemDTO items)
        {
            try
            {
                foreach (var item in items.Items)
                {
                    _context.TodoItems.Add(item);
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        //PUT: api/todoitems/5 + Body=item
        [HttpPut("{id}")]
        public async Task<ActionResult<TodoItem>> PutTodoItem(long id, TodoItem item)
        {
            if (id == item.Id)
            {
                _context.Entry(item).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("GetTodoItem", new { id = item.Id }, item);
                }
                catch (Exception)
                {
                    if (await _context.TodoItems.FindAsync(id) == null)
                    {
                        return NotFound();
                    }
                }
            }
            return BadRequest();
        }

        //DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem != null)
            {
                _context.TodoItems.Remove(todoItem);
                try
                {
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}
