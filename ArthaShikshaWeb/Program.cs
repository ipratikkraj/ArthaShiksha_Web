using ArthaShikshaClient;
using ArthaShikshaWeb.Services;
using ArthaShikshaWeb.Shared;
using Blazored.SessionStorage;
using LearniFyWeb.Services.AppService;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Radzen + Session storage
builder.Services.AddRadzenComponents();
builder.Services.AddBlazoredSessionStorage();

// Default HttpClient for general use (app origin)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Get the base API URL from configuration
var apiUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7003/";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiUrl)
});
builder.Services.AddScoped<IAppService, AppService>();

// Configure HttpClient with the correct base address
builder.Services.AddScoped(sp => new HttpClient {
    BaseAddress = new Uri("https://localhost:7003/")
});

// Add logging
builder.Services.AddLogging();

// Register AppService
builder.Services.AddScoped<IAppService, AppService>();
builder.Services.AddScoped<DialogService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMenuService, MenuService>();

// Build and run
await builder.Build().RunAsync();
