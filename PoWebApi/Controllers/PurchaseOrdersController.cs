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
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly PoContext _context;

        public PurchaseOrdersController(PoContext context)
        {
            _context = context;
        }

        // GET: api/PurchaseOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrders()
        {
            return await _context.PurchaseOrders
                                    .Include(e => e.Employee)
                                    .ToListAsync();
        }
        // GET: api/PurchaseOrders/underReview
        [HttpGet("underReview")]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrdersUnderReview()
        {
            return await _context.PurchaseOrders
                                    .Where(x => x.Status == PurchaseOrder.StatusReview)
                                    .Include(e => e.Employee)
                                    .ToListAsync();
        }
        // GET: api/PurchaseOrders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrder(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders
                                                .SingleOrDefaultAsync(e => e.Id == id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return purchaseOrder;
        }

        // GET: api/PurchaseOrders/5/getEmp
        [HttpGet("{id}/getEmp")]
        public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrderWithEmp(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders
                                                .Include(e => e.Employee)
                                                .SingleOrDefaultAsync(e => e.Id == id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return purchaseOrder;
        }

        // PUT: api/PurchaseOrders/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrder(int id, PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.Id)
            {
                return BadRequest();
            }

            _context.Entry(purchaseOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseOrderExists(id))
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

        // PUT: api/PurchaseOrders/5/edit
        [HttpPut("{id}/edit")]
        public async Task<IActionResult> PutPurchaseOrderToEdit(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            purchaseOrder.Status = "Edit";

            return await PutPurchaseOrder(id, purchaseOrder);
        }
        // PUT: api/PurchaseOrders/5/review
        [HttpPut("{id}/review")]
        public async Task<IActionResult> PutPurchaseOrderToReview(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            var total = purchaseOrder.Total;


            if (purchaseOrder == null)
            {
                return NotFound();
            }

            purchaseOrder.Status = (purchaseOrder.Total <= 100 && purchaseOrder.Total > 0) ? "Approved" : "Review";

            return await PutPurchaseOrder(id, purchaseOrder);

        }
        // PUT: api/PurchaseOrders/5/approve
        [HttpPut("{id}/approved")]
        public async Task<IActionResult> PutPurchaseOrderToApprove(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            purchaseOrder.Status = "Approved";

            return await PutPurchaseOrder(id, purchaseOrder);

        }
        // PUT: api/PurchaseOrders/5/reject
        [HttpPut("{id}/rejected")]
        public async Task<IActionResult> PutPurchaseOrderToReject(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            purchaseOrder.Status = "Rejected";

            return await PutPurchaseOrder(id, purchaseOrder);

        }
        //PUT: api/PurchaseOrders/5/status
        [HttpPut("manual/{id}/{status}")]
        public async Task<IActionResult> PutPurchaseOrderToStatus(int id, string status)
        {
            var statuses = "|edit| |review| |approved| |rejected|";
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            if (!statuses.Contains($"|{status.ToLower()}|"))
            {
                return BadRequest();
            }
            purchaseOrder.Status = char.ToUpper(status[0]) + status.Substring(1).ToLower() ;

            if(status.ToLower() == "review")
            {
                purchaseOrder.Status = (purchaseOrder.Total <= 100 && purchaseOrder.Total > 0) ? "Approved" : "Review";
            }
                return await PutPurchaseOrder(id, purchaseOrder);
        }

        // POST: api/PurchaseOrders
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<PurchaseOrder>> PostPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            _context.PurchaseOrders.Add(purchaseOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchaseOrder", new { id = purchaseOrder.Id }, purchaseOrder);
        }

        // DELETE: api/PurchaseOrders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PurchaseOrder>> DeletePurchaseOrder(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            _context.PurchaseOrders.Remove(purchaseOrder);
            await _context.SaveChangesAsync();

            return purchaseOrder;
        }

        private bool PurchaseOrderExists(int id)
        {
            return _context.PurchaseOrders.Any(e => e.Id == id);
        }
    }
}
