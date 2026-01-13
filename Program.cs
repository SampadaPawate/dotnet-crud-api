using Microsoft.EntityFrameworkCore;
using CrudApp.Data;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure port for Railway/cloud deployments
    // Railway sets PORT environment variable - use it if available, otherwise default to 8080
    var port = Environment.GetEnvironmentVariable("PORT");
    if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var portNumber))
    {
        builder.WebHost.UseUrls($"http://0.0.0.0:{portNumber}");
        Console.WriteLine($"Configured to listen on port: {portNumber} (from PORT env var)");
    }
    else
    {
        // Default to port 8080 for Railway
        builder.WebHost.UseUrls("http://0.0.0.0:8080");
        Console.WriteLine("Using default port: 8080 (PORT env var not set)");
    }

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    
    // Configure Swagger with safe XML comments loading
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Product CRUD API",
            Version = "v1",
            Description = "A RESTful API for managing products with full CRUD operations. Built with .NET 8, Entity Framework Core, and SQLite.",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "API Support",
                Email = "support@example.com"
            }
        });

        // Include XML comments in Swagger (safely, won't crash if missing)
        try
        {
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
                Console.WriteLine($"Loaded XML documentation from: {xmlPath}");
            }
            else
            {
                Console.WriteLine($"XML documentation file not found: {xmlPath} (Swagger will work without it)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not load XML documentation: {ex.Message}");
            // Continue without XML comments - Swagger will still work
        }
    });

    // Configure SQLite Database with error handling
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = "Data Source=products.db";
        Console.WriteLine("Using default connection string");
    }
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
    
    Console.WriteLine($"Database connection string: {connectionString}");

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    // Don't use HTTPS redirection in Railway (Railway handles HTTPS termination)
    
    // Add request logging for debugging
    app.Use(async (context, next) =>
    {
        try
        {
            Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
            await next();
            Console.WriteLine($"Response: {context.Response.StatusCode} for {context.Request.Path}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling request: {ex.Message}");
            throw;
        }
    });
    
    // Enable Swagger in all environments for portfolio/demo purposes
    app.UseSwagger();
    app.UseSwaggerUI();

    // Don't use UseAuthorization() without authentication - it can block requests
    // app.UseAuthorization(); // Commented out - not needed for public API
    app.MapControllers();

    // Root endpoint - return simple response instead of redirect
    app.MapGet("/", () => Results.Ok(new { 
        message = "API is running", 
        swagger = "/swagger",
        health = "/health",
        api = "/api/products"
    }));

    // Health check endpoint (Railway uses this to verify app is running)
    app.MapGet("/health", () => 
    {
        return Results.Ok(new { 
            status = "healthy", 
            timestamp = DateTime.UtcNow,
            port = Environment.GetEnvironmentVariable("PORT") ?? "5000"
        });
    });
    
    // Simple ping endpoint
    app.MapGet("/ping", () => Results.Ok("pong"));

    // Ensure database is created (non-blocking, with error handling)
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            Console.WriteLine("Database initialized successfully");
        }
    }
    catch (Exception ex)
    {
        // Log error but don't prevent app from starting
        Console.WriteLine($"Database initialization error: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        // App will continue without database - endpoints will handle errors
    }

    Console.WriteLine("Application starting...");
    Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
    Console.WriteLine($"Listening on: {string.Join(", ", app.Urls)}");
    Console.WriteLine("Application is ready to accept requests");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"FATAL ERROR during application startup: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
    }
    throw; // Re-throw to let Railway know the app failed
}
