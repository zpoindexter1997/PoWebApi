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
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderLinesController : ControllerBase
    {
        private readonly PoContext _context;

        public PurchaseOrderLinesController(PoContext context)
        {
            _context = context;
        }

        // GET: api/PurchaseOrderLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrderLine>>> GetPurchaseOrderLines()
        {
            return await _context.PurchaseOrderLines.ToListAsync();
        }

        // GET: api/PurchaseOrderLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrderLine>> GetPurchaseOrderLine(int id)
        {
            var purchaseOrderLine = await _context.PurchaseOrderLines.FindAsync(id);

            if (purchaseOrderLine == null)
            {
                return NotFound();
            }

            return purchaseOrderLine;
        }

        // PUT: api/PurchaseOrderLines/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrderLine(int id, PurchaseOrderLine purchaseOrderLine)
        {
            if (id != purchaseOrderLine.Id)
            {
                return BadRequest();
            }

            _context.Entry(purchaseOrderLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseOrderLineExists(id))
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

        // POST: api/PurchaseOrderLines
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<PurchaseOrderLine>> PostPurchaseOrderLine(PurchaseOrderLine purchaseOrderLine)
        {
            _context.PurchaseOrderLines.Add(purchaseOrderLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchaseOrderLine", new { id = purchaseOrderLine.Id }, purchaseOrderLine);
        }

        // DELETE: api/PurchaseOrderLines/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PurchaseOrderLine>> DeletePurchaseOrderLine(int id)
        {
            var purchaseOrderLine = await _context.PurchaseOrderLines.FindAsync(id);
            if (purchaseOrderLine == null)
            {
                return NotFound();
            }

            _context.PurchaseOrderLines.Remove(purchaseOrderLine);
            await _context.SaveChangesAsync();

            return purchaseOrderLine;
        }

        private bool PurchaseOrderLineExists(int id)
        {
            return _context.PurchaseOrderLines.Any(e => e.Id == id);
        }
    }
}
