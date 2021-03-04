using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Boatcam5.Data;
using Boatcam5.Models;

namespace Boatcam5.Pages.Cameras
{
    public class CreateModel : PageModel
    {
        private readonly Boatcam5.Data.ApplicationDbContext _context;

        public CreateModel(Boatcam5.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CameraPositions CameraPositions { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.CameraPositions.Add(CameraPositions);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
