using Microsoft.Extensions.Options;
using RoutePlanner.Domain;
namespace RoutePlanner.Tests
{
	public static class MockSettings
	{
		public static IOptions<RoutePlannerSettings> GetDefault()
		{
			return Options.Create(new RoutePlannerSettings
			{
				HomeLatitude = 29.7604,
				HomeLongitude = -95.3698,
				EarthRadiusKm = 6371,
				AverageSpeedKmph = 40
			});
		}

		public static IOptions<RoutePlannerSettings> GetCustom(
			double homeLat, double homeLng, double earthRadiusKm = 6371, double avgSpeed = 40)
		{
			return Options.Create(new RoutePlannerSettings
			{
				HomeLatitude = homeLat,
				HomeLongitude = homeLng,
				EarthRadiusKm = earthRadiusKm,
				AverageSpeedKmph = avgSpeed
			});
		}
	}
}