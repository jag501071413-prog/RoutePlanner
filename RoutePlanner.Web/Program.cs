using RoutePlanner.Application.Interfaces;
using RoutePlanner.Application.Services;
using RoutePlanner.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add configuration binding for RoutePlannerSettings
builder.Services.Configure<RoutePlannerSettings>(
    builder.Configuration.GetSection("RoutePlannerSettings"));

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IDistanceService, HaversineService>();
builder.Services.AddScoped<IRoutePlannerService, RoutePlannerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
