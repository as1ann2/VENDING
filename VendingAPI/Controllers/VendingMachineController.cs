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
    public class VendingMachineController : ControllerBase
    {
        private readonly VendingprofContext _context;

        public VendingMachineController(VendingprofContext context)
        {
            _context = context;
        }

        // GET: api/VendingMachine
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendingMachine>>> GetVendingMachines()
        {
            return await _context.VendingMachines.ToListAsync();
        }

        // GET: api/VendingMachine/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VendingMachine>> GetVendingMachine(int id)
        {
            var vendingMachine = await _context.VendingMachines.FindAsync(id);

            if (vendingMachine == null)
            {
                return NotFound();
            }

            return vendingMachine;
        }

        // PUT: api/VendingMachine/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendingMachine(int id, VendingMachine vendingMachine)
        {
            if (id != vendingMachine.MachineId)
            {
                return BadRequest();
            }

            _context.Entry(vendingMachine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendingMachineExists(id))
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

        // POST: api/VendingMachine
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VendingMachine>> PostVendingMachine(VendingMachine vendingMachine)
        {
            _context.VendingMachines.Add(vendingMachine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVendingMachine", new { id = vendingMachine.MachineId }, vendingMachine);
        }

        // DELETE: api/VendingMachine/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendingMachine(int id)
        {
            var vendingMachine = await _context.VendingMachines.FindAsync(id);
            if (vendingMachine == null)
            {
                return NotFound();
            }

            _context.VendingMachines.Remove(vendingMachine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VendingMachineExists(int id)
        {
            return _context.VendingMachines.Any(e => e.MachineId == id);
        }
    }
}
