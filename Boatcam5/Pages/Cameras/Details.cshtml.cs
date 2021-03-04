﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Boatcam5.Data;
using Boatcam5.Models;

namespace Boatcam5.Pages.Cameras
{
    public class DetailsModel : PageModel
    {
        private readonly Boatcam5.Data.ApplicationDbContext _context;

        public DetailsModel(Boatcam5.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
            return Page();
        }
    }
}