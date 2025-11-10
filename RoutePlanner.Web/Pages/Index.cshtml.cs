using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoutePlanner.Application.Interfaces;
using RoutePlanner.Domain;
using System.Text.Json;

namespace RoutePlanner.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IRoutePlannerService _routePlanner;

        public IndexModel(IRoutePlannerService routePlanner)
        {
            _routePlanner = routePlanner;
        }

        [BindProperty]
        public string? JsonInput { get; set; }

        public RouteResult? Result { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Load sample JSON by default into textarea for convenience
            var samplePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "sample-data", "sample-appointments.json");
            if (System.IO.File.Exists(samplePath))
            {
                JsonInput = System.IO.File.ReadAllText(samplePath);
            }
        }

        public async Task<IActionResult> OnPostAsync(Microsoft.AspNetCore.Http.IFormFile? JsonFile, string? action)
        {
            if (string.Equals(action, "loadsample", StringComparison.OrdinalIgnoreCase))
            {
                var samplePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "sample-data", "sample-appointments.json");
                if (System.IO.File.Exists(samplePath))
                {
                    JsonInput = System.IO.File.ReadAllText(samplePath);
                }
            }
            else
            {
                if (JsonFile != null)
                {
                    using var reader = new StreamReader(JsonFile.OpenReadStream());
                    JsonInput = await reader.ReadToEndAsync();
                }
            }

            if (string.IsNullOrWhiteSpace(JsonInput))
            {
                ErrorMessage = "Please provide JSON input (paste or upload). Use 'Load Sample Data' if you need an example.";
                return Page();
            }

            try
            {
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var appointments = JsonSerializer.Deserialize<List<Appointment>>(JsonInput, opts);
                if (appointments == null)
                {
                    ErrorMessage = "Unable to parse JSON into appointments.";
                    return Page();
                }

                // Basic validation
                foreach (var a in appointments)
                {
                    if (a.WindowEnd < a.WindowStart)
                    {
                        ErrorMessage = $"Invalid time window for patient {a.PatientId} / {a.PatientName}. WindowEnd is before WindowStart.";
                        return Page();
                    }
                }

                Result = _routePlanner.BuildRoute(appointments, null, null);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error processing input: " + ex.Message;
            }

            return Page();
        }
    }
}
