using NUnit.Framework;
using RoutePlanner.Application.Interfaces;
using RoutePlanner.Application.Services;
using RoutePlanner.Domain;
namespace RoutePlanner.Tests
{
	[TestFixture]
	public class RoutePlannerServiceTests
	{
		private IRoutePlannerService _planner;
		[SetUp]
		public void Setup()
		{
			var settings = MockSettings.GetDefault();
			var distanceService = new HaversineService(settings);
			_planner = new RoutePlannerService(distanceService, settings);
		}
		[Test]
		public void BuildRoute_SimpleFeasibleSchedule_ReturnsAllStops()
		{
			var appointments = new List<Appointment>
			{
				new Appointment
				{
					PatientId = 1,
					PatientName = "A",
					Latitude = 29.7620,
					Longitude = -95.3670,
					WindowStart = DateTime.Parse("2025-11-10T09:00:00"),
					WindowEnd = DateTime.Parse("2025-11-10T10:00:00"),
					DurationMinutes = 30
				},
				new Appointment
				{
					PatientId = 2,
					PatientName = "B",
					Latitude = 29.7500,
					Longitude = -95.3600,
					WindowStart = DateTime.Parse("2025-11-10T11:00:00"),
					WindowEnd = DateTime.Parse("2025-11-10T12:00:00"),
					DurationMinutes = 45
				}
			};
			var result = _planner.BuildRoute(appointments, 29.7604, -95.3698);
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Stops.Count, Is.EqualTo(4)); // Start + 2 visits + End
			Assert.That(result.TotalDistanceKm, Is.GreaterThan(0));
			Assert.That(result.TotalDuration.TotalMinutes, Is.GreaterThan(0));
		}
		[Test]
		public void BuildRoute_NoAppointments_ReturnsOnlyHome()
		{
			var result = _planner.BuildRoute(new List<Appointment>(), 29.7604, -95.3698);
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Stops.Count, Is.EqualTo(1));
			Assert.That(result.TotalDistanceKm, Is.EqualTo(0));
		}
		[Test]
		public void BuildRoute_UnreachableAppointment_IsHandledGracefully()
		{
			var appointments = new List<Appointment>
			{
				new Appointment
				{
					PatientId = 99,
					PatientName = "Impossible",
					Latitude = 0, Longitude = 0,
					WindowStart = DateTime.Parse("2020-01-01T09:00:00"),
					WindowEnd = DateTime.Parse("2020-01-01T09:15:00"),
					DurationMinutes = 15
				}
			};
			var result = _planner.BuildRoute(appointments, 29.7604, -95.3698);
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Stops.Count, Is.GreaterThanOrEqualTo(1));
		}
	}
}