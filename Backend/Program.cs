var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:7142/") // Your Blazor app URL
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ... other services

var app = builder.Build();

app.UseCors("BlazorApp");
// ... other middleware