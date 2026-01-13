using Microsoft.EntityFrameworkCore;
using CrudApp.Data;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure port for Railway/cloud deployments
    var port = Environment.GetEnvironmentVariable("PORT");
    if (!string.IsNullOrEmpty(port) && int.TryParse(port, out var portNumber))
    {
        builder.WebHost.UseUrls($"http://0.0.0.0:{portNumber}");
        Console.WriteLine($"Configured to listen on port: {portNumber}");
    }
    else
    {
        // Default to port 5000 for local development
        builder.WebHost.UseUrls("http://0.0.0.0:5000");
        Console.WriteLine("Using default port: 5000");
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
    // Enable Swagger in all environments for portfolio/demo purposes
    app.UseSwagger();
    app.UseSwaggerUI();

    // Only redirect to HTTPS if both HTTP and HTTPS are configured
    var httpsPort = app.Configuration["ASPNETCORE_HTTPS_PORT"];
    if (!string.IsNullOrEmpty(httpsPort))
    {
        app.UseHttpsRedirection();
    }

    app.UseAuthorization();
    app.MapControllers();

    // Redirect root URL to Swagger
    app.MapGet("/", () => Results.Redirect("/swagger"));

    // Health check endpoint
    app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

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
