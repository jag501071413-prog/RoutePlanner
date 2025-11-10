using Microsoft.Extensions.Options;
using RoutePlanner.Application.Interfaces;
using RoutePlanner.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutePlanner.Application.Services
{
    public class HaversineService : IDistanceService
    {
        private readonly double _earthRadiusKm;

        public HaversineService(IOptions<RoutePlannerSettings> settings)
        {
            _earthRadiusKm = settings.Value.EarthRadiusKm;
        }
        public double GetDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Pow(Math.Sin(dLon / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return _earthRadiusKm * c;
        }

        private static double ToRadians(double angle) => Math.PI * angle / 180.0;
    }
}
