using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OnvifCamera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boatcam5.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ICamera camera;

        public IndexModel(ILogger<IndexModel> logger /*, ICamera camera*/)
        {
            _logger = logger;
            //this.camera = camera;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {

            //camera.MoveTo(new PtzValue { X = 180, Y = 0, Zoom = 1 });


            return RedirectToPage("./Index");
        }

    }
}
