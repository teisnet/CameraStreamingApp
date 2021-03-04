using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Boatcam5.Data;
using Boatcam5.Models;
using Onvif.Core.Client;
using Onvif.Core.Client.Common;

namespace Boatcam5.Pages.Cameras
{
    public class EditModel : PageModel
    {
        private readonly Boatcam5.Data.ApplicationDbContext _context;

        public EditModel(Boatcam5.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CameraPositions CameraPositions { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CameraPositions = await _context.CameraPositions.FirstOrDefaultAsync(m => m.Id == id);


            if (CameraPositions == null)
            {
                return NotFound();
            }

            CameraPositions.X = 0f;
            CameraPositions.Y = 0f;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var account = new Account("192.168.1.9", "admin", "123456");
            var camera = Camera.Create(account, ex =>
            {
                // exception
            });
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CameraPositions).State = EntityState.Modified;

            try
            {

                var pTZStatus = await camera.Ptz.GetStatusAsync("MediaProfile000");

                CameraPositions.X = pTZStatus.Position.PanTilt.x;
                CameraPositions.Y = pTZStatus.Position.PanTilt.y;
                //CameraPositions.Y = pTZStatus.Position.PanTilt.y;
                CameraPositions.Z = pTZStatus.Position.Zoom.x;



                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CameraPositionsExists(CameraPositions.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CameraPositionsExists(int id)
        {
            return _context.CameraPositions.Any(e => e.Id == id);
        }
    }
}
