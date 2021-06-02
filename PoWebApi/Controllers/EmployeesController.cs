using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PoWebApi.Data;
using PoWebApi.Models;

namespace PoWebApi.Controllers
{
    //Route points to the site we'd use to access this controller
    //http://localhost:7467/api/Employees
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        //creates our context that can only be set in one method and readonly afterwards
        private readonly PoContext _context;
        //the one method setting our context
        public EmployeesController(PoContext context)
        {
            _context = context;
        }

        //GET: api/Employees/gpdoud/passord
        [HttpGet("{login}/{password}")]
        public async Task<ActionResult<Employee>> Login (string login, string password)
        {
            var employee = await _context.Employee
                .SingleOrDefaultAsync(e => e.Login == login && e.Password == password);
            if(employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        // GET: api/Employees
        [HttpGet]
        //async -> anything that calls this, must call it asynchronously
        //Task is a class in .NET that is used for Async data
        //returns ActionResult -> class that has lots of derived classes that you can use to return diff things to use in your methods/primarily for returning error messages
        //if ActionResult catches no errors, returns IEnumerable -> interface for a collection of <Employee>
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            //must use await before the context to do things asynchronously, allows us to do async calls as if they were synchronous
            return await _context.Employee.ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employee.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Employees
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            //only time you don't need await before context is adding
            _context.Employee.Add(employee);
            //still need it for save changes
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }

    }
}
