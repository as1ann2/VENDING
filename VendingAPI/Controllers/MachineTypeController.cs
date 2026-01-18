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
    public class MachineTypeController : ControllerBase
    {
        private readonly VendingprofContext _context;

        public MachineTypeController(VendingprofContext context)
        {
            _context = context;
        }

        // GET: api/MachineType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MachineType>>> GetMachineTypes()
        {
            return await _context.MachineTypes.ToListAsync();
        }

        // GET: api/MachineType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MachineType>> GetMachineType(int id)
        {
            var machineType = await _context.MachineTypes.FindAsync(id);

            if (machineType == null)
            {
                return NotFound();
            }

            return machineType;
        }

        // PUT: api/MachineType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMachineType(int id, MachineType machineType)
        {
            if (id != machineType.TypeId)
            {
                return BadRequest();
            }

            _context.Entry(machineType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MachineTypeExists(id))
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

        // POST: api/MachineType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MachineType>> PostMachineType(MachineType machineType)
        {
            _context.MachineTypes.Add(machineType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMachineType", new { id = machineType.TypeId }, machineType);
        }

        // DELETE: api/MachineType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachineType(int id)
        {
            var machineType = await _context.MachineTypes.FindAsync(id);
            if (machineType == null)
            {
                return NotFound();
            }

            _context.MachineTypes.Remove(machineType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MachineTypeExists(int id)
        {
            return _context.MachineTypes.Any(e => e.TypeId == id);
        }
    }
}
