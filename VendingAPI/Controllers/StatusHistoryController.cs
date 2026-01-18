using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingAPI.Models;

namespace VendingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusHistoryController : ControllerBase
    {
        private readonly VendingprofContext _context;

        public StatusHistoryController(VendingprofContext context)
        {
            _context = context;
        }

        // GET: api/StatusHistory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusHistory>>> GetStatusHistories()
        {
            return await _context.StatusHistories.ToListAsync();
        }

        // GET: api/StatusHistory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StatusHistory>> GetStatusHistory(int id)
        {
            var statusHistory = await _context.StatusHistories.FindAsync(id);

            if (statusHistory == null)
            {
                return NotFound();
            }

            return statusHistory;
        }

        // PUT: api/StatusHistory/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStatusHistory(int id, StatusHistory statusHistory)
        {
            if (id != statusHistory.HistoryId)
            {
                return BadRequest();
            }

            _context.Entry(statusHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StatusHistoryExists(id))
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

        // POST: api/StatusHistory
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StatusHistory>> PostStatusHistory(StatusHistory statusHistory)
        {
            _context.StatusHistories.Add(statusHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStatusHistory", new { id = statusHistory.HistoryId }, statusHistory);
        }

        // DELETE: api/StatusHistory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatusHistory(int id)
        {
            var statusHistory = await _context.StatusHistories.FindAsync(id);
            if (statusHistory == null)
            {
                return NotFound();
            }

            _context.StatusHistories.Remove(statusHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StatusHistoryExists(int id)
        {
            return _context.StatusHistories.Any(e => e.HistoryId == id);
        }
    }
}
