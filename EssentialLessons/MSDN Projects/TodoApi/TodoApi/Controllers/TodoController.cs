using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        // Определяет класс контроллера API без методов.

        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
            if (_context.TodoItems.Count() ==0)
            {
                // Create a new TodoItem if collection is empty,
                // which means you can't delete all TodoItems.

                _context.TodoItems.Add(new TodoItem { Name = "Item1" });
                _context.SaveChanges();

            }
        }
    }
}
