using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Boatcam5.Data;
using Boatcam5.Models;
using Microsoft.AspNetCore.Authorization;

namespace Boatcam5.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CameraPositionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CameraPositionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CameraPositions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CameraPositions>>> GetCameraPositions()
        {
            return await _context.CameraPositions.ToListAsync();
        }

        // GET: api/CameraPositions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CameraPositions>> GetCameraPositions(int id)
        {
            var cameraPositions = await _context.CameraPositions.FindAsync(id);

            if (cameraPositions == null)
            {
                return NotFound();
            }

            return cameraPositions;
        }

        // PUT: api/CameraPositions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCameraPositions(int id, CameraPositions cameraPositions)
        {
            if (id != cameraPositions.Id)
            {
                return BadRequest();
            }

            _context.Entry(cameraPositions).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CameraPositionsExists(id))
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

        // POST: api/CameraPositions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CameraPositions>> PostCameraPositions(CameraPositions cameraPositions)
        {
            _context.CameraPositions.Add(cameraPositions);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCameraPositions", new { id = cameraPositions.Id }, cameraPositions);
        }

        // DELETE: api/CameraPositions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCameraPositions(int id)
        {
            var cameraPositions = await _context.CameraPositions.FindAsync(id);
            if (cameraPositions == null)
            {
                return NotFound();
            }

            _context.CameraPositions.Remove(cameraPositions);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CameraPositionsExists(int id)
        {
            return _context.CameraPositions.Any(e => e.Id == id);
        }
    }
}
