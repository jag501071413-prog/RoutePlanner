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
    public class RoutePlannerService : IRoutePlannerService
    {
        private readonly IDistanceService _distanceService;
        private readonly RoutePlannerSettings _settings;

        public RoutePlannerService(IDistanceService distanceService, IOptions<RoutePlannerSettings> settings)
        {
            _distanceService = distanceService;
            _settings = settings.Value;
        }
        public RouteResult BuildRoute(List<Appointment> appointments, double? homeLat = null, double? homeLon = null)
        {
            // Defensive copy and basic validation
            var remaining = appointments.OrderBy(a => a.WindowStart).ToList();
            var result = new RouteResult();

            if (!remaining.Any())
            {
                // Start and end at home immediately
                var now = DateTime.Today.AddHours(8); // default start of day
                result.Stops.Add(new RouteStop
                {
                    LocationName = "Home (Start/End)",
                    Latitude = _settings.HomeLatitude,
                    Longitude = _settings.HomeLongitude,
                    ArrivalTime = now,
                    DepartureTime = now,
                    TravelDistanceKm = 0
                });
                result.TotalDistanceKm = 0;
                result.TotalDuration = TimeSpan.Zero;
                return result;
            }

            // Start time: earliest appointment window start minus small buffer
            DateTime currentTime = remaining.Min(a => a.WindowStart).AddMinutes(-30);
            double currentLat = _settings.HomeLatitude, currentLon = _settings.HomeLongitude;
            double totalDistance = 0;

            result.Stops.Add(new RouteStop
            {
                LocationName = "Home (Start)",
                Latitude = _settings.HomeLatitude,
                Longitude = _settings.HomeLongitude,
                ArrivalTime = currentTime,
                DepartureTime = currentTime,
                TravelDistanceKm = 0
            });

            while (remaining.Any())
            {
                Appointment? best = null;
                double bestScore = double.MaxValue;
                double bestDist = 0;
                DateTime bestArrival = DateTime.MinValue;

                foreach (var appt in remaining)
                {
                    double dist = _distanceService.GetDistanceKm(currentLat, currentLon, appt.Latitude, appt.Longitude);
                    double travelMins = dist / _settings.AverageSpeedKmph * 60.0;
                    var tentativeArrival = currentTime.AddMinutes(travelMins);

                    // If tentative arrival is after window end, it's infeasible
                    if (tentativeArrival > appt.WindowEnd) continue;

                    // If early, doctor can wait until window start
                    var arrival = tentativeArrival < appt.WindowStart ? appt.WindowStart : tentativeArrival;
                    var waitMins = (arrival - tentativeArrival).TotalMinutes;
                    // Heuristic score: prefer small wait and short distance
                    double score = waitMins + dist * 2.0;

                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = appt;
                        bestDist = dist;
                        bestArrival = arrival;
                    }
                }

                if (best == null)
                {
                    // No feasible next appointment found -> break and fail gracefully
                    break;
                }

                // Add to route
                double travelMinutes = bestDist / _settings.AverageSpeedKmph * 60.0;
                DateTime arrivalTime = currentTime.AddMinutes(travelMinutes);
                if (arrivalTime < best.WindowStart) arrivalTime = best.WindowStart;
                DateTime departureTime = arrivalTime.AddMinutes(best.DurationMinutes);

                result.Stops.Add(new RouteStop
                {
                    LocationName = $"{best.PatientName} (ID:{best.PatientId})",
                    Latitude = best.Latitude,
                    Longitude = best.Longitude,
                    TravelDistanceKm = Math.Round(bestDist, 3),
                    ArrivalTime = arrivalTime,
                    DepartureTime = departureTime
                });

                totalDistance += bestDist;
                currentTime = departureTime;
                currentLat = best.Latitude;
                currentLon = best.Longitude;
                remaining.Remove(best);
            }

            // Return home
            double returnDist = _distanceService.GetDistanceKm(currentLat, currentLon, _settings.HomeLatitude, _settings.HomeLongitude);
            totalDistance += returnDist;
            DateTime finalArrival = currentTime.AddMinutes(returnDist / _settings.AverageSpeedKmph * 60.0);

            result.Stops.Add(new RouteStop
            {
                LocationName = "Home (End)",
                Latitude = _settings.HomeLatitude,
                Longitude = _settings.HomeLongitude,
                TravelDistanceKm = Math.Round(returnDist, 3),
                ArrivalTime = finalArrival,
                DepartureTime = finalArrival
            });

            result.TotalDistanceKm = Math.Round(totalDistance, 3);
            result.TotalDuration = finalArrival - result.Stops.First().ArrivalTime;
            return result;
        }
    }
}
