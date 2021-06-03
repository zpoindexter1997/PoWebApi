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
    //The 7149 is your port number
    //http://localhost:7149/api/Employees
    [Route("api/[controller]")]
    //ApiController attribute takes the class name of our controller and strips off the word Controller (leaving us with Employees)
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        //creates our context variable - readonly means we can only change it in a constructor, and afterwards it'll only be readable
        private readonly PoContext _context;
        //The constructor for context, takes whatever context we pass in and sets it to _context to be used in this class (dependency injection)
        public EmployeesController(PoContext context)
        {
            _context = context;
        }


        // GET: api/Employees
        // GET = SELECT
        [HttpGet]
        //async -> anything that calls this, must call it asynchronously (without preventing the user from doing anything else)
        //Task is a class in .NET that is required for Async data
        //returns ActionResult -> class that has lots of derived classes (Bad Request, Not Found, etc..) that you can use to return diff things to use in your methods/primarily for returning error messages
        //if ActionResult catches no errors, returns IEnumerable -> interface for a collection of <Employee>, which is the generic collection class (so we can return an array or a list)
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            //must use await before the context to do things asynchronously, allows us to do async calls as if they were synchronous
            return await _context.Employee.ToListAsync();
        }

        // GET: api/Employees/5
        //("{id}") is a route parameter in the HttpGet, requiring us to add /? to our url where ? is the data that gets loaded into id
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employee.FindAsync(id);

            //If we didn't find an employee, we can do this instead of returning null
            if (employee == null)
            {
                //this return is acceptable because it is an ActionResult type
                return NotFound();
            }
            //returns the Ok ActionResult type, as well as the data for (employee)
            return Ok(employee);
        }

        //GET: api/Employees/gpdoud/password
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

        // PUT: api/Employees/5
        // PUT = UPDATE
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
        //POST = INSERT
        [HttpPost]
        //A post method is expecting us to pass the entire instance of the data (employee) into the body of the request, not the url
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            //only time you don't need await before context is adding because it is just loading the data into a cache
            _context.Employee.Add(employee);

            //still need it for save changes
            await _context.SaveChangesAsync();
            //Returns CreatedAtAction ActionResult type, with data ("GetEmployee"(method to show us the employee that was added), with the new employee instance)
            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        // DELETE: api/Employees/5
        // DELETE = DELETE
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
