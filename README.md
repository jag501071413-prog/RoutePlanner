# RoutePlanner

RoutePlanner is a .NET 8 web application designed to optimize routes for medical professionals who make house calls. The application calculates the most efficient route between multiple patient appointments based on geographic coordinates and time windows.

## Overview

RoutePlanner is built as a multi-layered ASP.NET Core application that:

- Accepts appointment data in JSON format with patient locations and time windows
- Uses geographic coordinates to calculate distances using the Haversine formula
- Implements a heuristic algorithm to optimize the route based on travel time and appointment windows
- Generates a complete route plan starting from and returning to a home base location

## Features

- **Geographic Route Optimization**: Uses latitude and longitude coordinates to calculate real-world distances between appointments
- **Time Window Management**: Respects appointment time windows to ensure doctors arrive within acceptable time frames
- **JSON Input/Output**: Handles appointment data in JSON format with options to upload files or paste directly
- **Web Interface**: Simple web-based UI built with Razor Pages for easy access and use
- **Comprehensive Route Details**: Provides arrival/departure times, travel distances, and total route metrics

## Architecture

The application follows a clean architecture pattern with the following layers:

- **Domain**: Contains core business entities like `Appointment`, `RouteResult`, `RouteStop`, and configuration settings
- **Application**: Houses business logic interfaces and services including the route planning algorithm
- **Infrastructure**: Reserved for data access, external services, and infrastructure concerns
- **Web**: ASP.NET Core Web Application with Razor Pages UI and API endpoints
- **Tests**: Unit and integration tests for all critical functionality

## Technology Stack

- **Backend**: ASP.NET Core 8 with .NET 8
- **Frontend**: Razor Pages with Bootstrap CSS
- **Algorithm**: Haversine formula for distance calculation and custom heuristic for route optimization
- **Dependency Injection**: Built-in .NET DI container
- **Configuration**: JSON-based configuration with strongly-typed settings

## How It Works

1. **Input**: Users provide appointment data in JSON format containing:
   - Patient ID and name
   - Geographic coordinates (latitude, longitude)
   - Appointment time window (start/end times)
   - Appointment duration

2. **Processing**: The application:
   - Calculates distances between all appointments using the Haversine formula
   - Considers travel time based on average speed settings
   - Optimizes the route by selecting the next feasible appointment with the best score (considering wait time and travel distance)

3. **Output**: Generates a complete route plan with:
   - Sequential stops with arrival/departure times
   - Travel distances between stops
   - Total route distance and duration

## Configuration

The application can be configured via `appsettings.json`:

```json
{
  "RoutePlannerSettings": {
    "HomeLatitude": 29.7604,        // Base location latitude (Houston, TX in default)
    "HomeLongitude": -95.3698,      // Base location longitude
    "EarthRadiusKmph": 6371,        // Earth radius in kilometers
    "AverageSpeedKmph": 40          // Average travel speed in km/h
  }
}
```

## Usage

1. Launch the web application
2. Either paste appointment JSON data directly or upload a JSON file
3. Click "Upload & Plan Route" to generate the optimized route
4. View the complete route plan with all stops and timing details

## Algorithm Details

The route optimization algorithm uses a heuristic approach:
- It prioritizes appointments based on the earliest time window start
- For each step, it calculates the travel time to all remaining appointments
- It selects the next appointment that minimizes a score combining wait time and travel distance
- The algorithm ensures all appointments are visited within their time windows

## Project Status

This is a complete, working application suitable for planning daily house calls for medical professionals. The application handles edge cases like infeasible appointments and provides comprehensive route information.

## Testing

Unit tests are included in the RoutePlanner.Tests project to ensure the reliability of the routing algorithm and distance calculations.
