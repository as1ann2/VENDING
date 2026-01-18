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
    public class MachineInventoryController : ControllerBase
    {
        private readonly VendingprofContext _context;

        public MachineInventoryController(VendingprofContext context)
        {
            _context = context;
        }

        // GET: api/MachineInventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MachineInventory>>> GetMachineInventories()
        {
            return await _context.MachineInventories.ToListAsync();
        }

        // GET: api/MachineInventory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MachineInventory>> GetMachineInventory(int id)
        {
            var machineInventory = await _context.MachineInventories.FindAsync(id);

            if (machineInventory == null)
            {
                return NotFound();
            }

            return machineInventory;
        }

        // PUT: api/MachineInventory/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMachineInventory(int id, MachineInventory machineInventory)
        {
            if (id != machineInventory.InventoryId)
            {
                return BadRequest();
            }

            _context.Entry(machineInventory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MachineInventoryExists(id))
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

        // POST: api/MachineInventory
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MachineInventory>> PostMachineInventory(MachineInventory machineInventory)
        {
            _context.MachineInventories.Add(machineInventory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMachineInventory", new { id = machineInventory.InventoryId }, machineInventory);
        }

        // DELETE: api/MachineInventory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineInventory(int id)
        {
            var machineInventory = await _context.MachineInventories.FindAsync(id);
            if (machineInventory == null)
            {
                return NotFound();
            }

            _context.MachineInventories.Remove(machineInventory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MachineInventoryExists(int id)
        {
            return _context.MachineInventories.Any(e => e.InventoryId == id);
        }
    }
}
