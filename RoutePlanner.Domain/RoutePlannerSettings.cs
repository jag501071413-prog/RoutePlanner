using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutePlanner.Domain
{
    public class RoutePlannerSettings
    {
        public double HomeLatitude { get; set; }
        public double HomeLongitude { get; set; }
        public double EarthRadiusKm { get; set; }
        public double AverageSpeedKmph { get; set; }
    }
}
