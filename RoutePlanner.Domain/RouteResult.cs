using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutePlanner.Domain
{
    public class RouteResult
    {
        public List<RouteStop> Stops { get; set; } = new();
        public double TotalDistanceKm { get; set; }
        public TimeSpan TotalDuration { get; set; }
    }
}
