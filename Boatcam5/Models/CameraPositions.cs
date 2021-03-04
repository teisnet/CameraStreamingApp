using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boatcam5.Models
{
    public class CameraPositions
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
