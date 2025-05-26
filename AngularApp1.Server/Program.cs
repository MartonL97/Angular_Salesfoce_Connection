using AngularApp1.Server.Data;
using AngularApp1.Server.Interfaces;
using AngularApp1.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Logging.ApplicationInsights;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews(); // Use MVC with views

// Register TokenStore as Singleton to share the same instance
builder.Services.AddSingleton<TokenStore>();

// Register the custom Salesforce service
builder.Services.AddScoped<ISalesforceService, SalesforceService>();
builder.Services.AddScoped<ISalesforceAuthService, SalesforceAuthorizationService>();

// Register TokenService
builder.Services.AddSingleton<TokenService>();

// JWT Authentication Setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"] ?? string.Empty))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PlayerOnly", policy => policy.RequireRole("Player"));
    options.AddPolicy("CoachOnly", policy => policy.RequireRole("Coach"));
});

builder.Services.AddAuthorization();


// Add services to the container.
builder.Services.AddControllers();

// 🔹 Add Application Insights
builder.Logging.AddApplicationInsights(
    configureTelemetryConfiguration: (config) =>
        config.ConnectionString = builder.Configuration.GetConnectionString("InstrumentationKey=4ea0bf97-e9b8-44f4-9947-ce27ca18d96c;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=2829b9e9-6985-4b16-af26-b3d521f7fd94"),
    configureApplicationInsightsLoggerOptions: (options) => { }
);

builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("*", LogLevel.Trace);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

// Enable Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToFile("/index.html");

app.Run();
