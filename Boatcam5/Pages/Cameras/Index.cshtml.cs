using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Boatcam5.Data;
using Boatcam5.Models;
using Onvif.Core.Client;
using Onvif.Core.Client.Common;

namespace Boatcam5.Pages.Cameras
{
    public class IndexModel : PageModel
    {
        private readonly Boatcam5.Data.ApplicationDbContext _context;

        public IndexModel(Boatcam5.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<CameraPositions> CameraPositions { get; set; }

        public async Task OnGetAsync()
        {
            CameraPositions = await _context.CameraPositions.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CameraPositions CameraPosition = await _context.CameraPositions.FindAsync(id);

            if (CameraPosition != null)
            {
                MoveCamera(CameraPosition);
            }

            return RedirectToPage("./Index");
        }


        public async Task MoveCamera(CameraPositions CameraPosition)
        {
            var account = new Account("192.168.1.9", "admin", "123456");
            var camera = Camera.Create(account, ex =>
            {
                // exception
            });

            if (camera != null)
            {
                //move...
                
                
                var vector1 = new PTZVector {
                    PanTilt = new Vector2D { x =  CameraPosition.X, y= CameraPosition.Y },
                    Zoom = new Vector1D { x = CameraPosition.Z }
                };
                var speed1 = new PTZSpeed { PanTilt = new Vector2D { x = 0, y = 0 },
                    Zoom = new Vector1D { x = 0 }
                };
                await camera.MoveAsync(MoveType.Absolute, vector1, speed1, 0);
                
                
                var vectorContinuas = new PTZVector
                {
                    PanTilt = new Vector2D { x = 1, y = 0 },
                    Zoom = new Vector1D { x = 0 }
                };


                var test =  await camera.Ptz.GetNodesAsync();

               

                try
                {
                    var profile = camera.Profile; 
                    //var test2 = await camera.Ptz.GetStatusAsync("ProfileA01:00");
                    var test2 = await camera.Ptz.GetStatusAsync("MediaProfile000");
                    //var test3 = await camera.Ptz.Get("ProfileA01:00");
                    

                }
                catch (Exception e)
                {

                    throw;
                }
               




                //zoom...
                //var vector2 = new PTZVector { Zoom = new Vector1D { x = CameraPosition.Z } };
                //var speed2 = new PTZSpeed { Zoom = new Vector1D { x = 0 } };
                //camera.MoveAsync(MoveType.Absolute, vector2, speed2, 0);

            }


        }
    }
}


    

