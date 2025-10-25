using Microsoft.EntityFrameworkCore;
using PruebaAPI.Application.Interfaces;
using PruebaAPI.Application.Services;
using PruebaAPI.Domain.Interfaces;
using PruebaAPI.Infrastructure.Data;
using PruebaAPI.Infrastructure.Persistence;
using PruebaAPI.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Infrastructure Layer - DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Infrastructure Layer - Repositories and Unit of Work
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IMarcaAutoRepository, MarcaAutoRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application Layer - Services
builder.Services.AddScoped<IMarcaAutoService, MarcaAutoService>();

// Presentation Layer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "PruebaAPI - Marcas de Autos", 
        Version = "v2.0",
        Description = "API de Marcas de Autos con Clean Architecture"
    });
});

var app = builder.Build();

// Apply migrations and seed data automatically
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        // Aplicar migraciones
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully");
        
        // Seed data - SOLO EN DESARROLLO
        if (app.Environment.IsDevelopment())
        {
            await DbSeeder.SeedAsync(context);
            logger.LogInformation("Database seeded successfully (Development environment)");
        }
        else
        {
            logger.LogInformation("Skipping database seed - Production environment");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or seeding the database");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

