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
    public class CashCollectionController : ControllerBase
    {
        private readonly VendingprofContext _context;

        public CashCollectionController(VendingprofContext context)
        {
            _context = context;
        }

        // GET: api/CashCollection
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CashCollection>>> GetCashCollections()
        {
            return await _context.CashCollections.ToListAsync();
        }

        // GET: api/CashCollection/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CashCollection>> GetCashCollection(int id)
        {
            var cashCollection = await _context.CashCollections.FindAsync(id);

            if (cashCollection == null)
            {
                return NotFound();
            }

            return cashCollection;
        }

        // PUT: api/CashCollection/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCashCollection(int id, CashCollection cashCollection)
        {
            if (id != cashCollection.CollectionId)
            {
                return BadRequest();
            }

            _context.Entry(cashCollection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CashCollectionExists(id))
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

        // POST: api/CashCollection
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CashCollection>> PostCashCollection(CashCollection cashCollection)
        {
            _context.CashCollections.Add(cashCollection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCashCollection", new { id = cashCollection.CollectionId }, cashCollection);
        }

        // DELETE: api/CashCollection/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCashCollection(int id)
        {
            var cashCollection = await _context.CashCollections.FindAsync(id);
            if (cashCollection == null)
            {
                return NotFound();
            }

            _context.CashCollections.Remove(cashCollection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CashCollectionExists(int id)
        {
            return _context.CashCollections.Any(e => e.CollectionId == id);
        }
    }
}
