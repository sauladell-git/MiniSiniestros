using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Extensions;
using MiniSiniestros.Data.Migrations.Seeds;
using MiniSiniestros.Services.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog Host
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Add services to the container.
builder.Services.AddControllers();

// Configure EF Core Data Services (DbContext, Repositories, UnitOfWork)
builder.Services.AddDataServices(builder.Configuration);

// Configure Business Services Layer
builder.Services.AddServiceLayer();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Serilog HTTP Request Logging
app.UseSerilogRequestLogging();

// Automatically apply pending database migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MiniSiniestrosDbContext>();
        await DbInitializer.InitializeAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al aplicar migraciones o inicializar la base de datos.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
