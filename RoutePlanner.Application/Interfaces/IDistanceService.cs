using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutePlanner.Application.Interfaces
{
    public interface IDistanceService
    {
        double GetDistanceKm(double lat1, double long1, double lat2, double long2);
    }
}
