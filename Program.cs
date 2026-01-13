using Microsoft.EntityFrameworkCore;
using CrudApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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

    // Include XML comments in Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure SQLite Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
