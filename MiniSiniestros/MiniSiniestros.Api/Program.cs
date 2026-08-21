using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MiniSiniestros.Api.Handlers;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Extensions;
using MiniSiniestros.Data.Migrations.Seeds;
using MiniSiniestros.Services.Extensions;
using MiniSiniestros.Services.Implementations;
using MiniSiniestros.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog Host
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Register Global Exception Handler (.NET 8 IExceptionHandler)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add services to the container.
builder.Services.AddControllers();

// Configure JWT Authentication
var secretKey = builder.Configuration["JwtSettings:Secret"] ?? "MiniSiniestrosSuperSecretKeyForJWTAuthToken2026!MustBeLongEnough";
var issuer = builder.Configuration["JwtSettings:Issuer"] ?? "MiniSiniestrosApi";
var audience = builder.Configuration["JwtSettings:Audience"] ?? "MiniSiniestrosApp";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Configure Role Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("RequireOperadorRole", policy => policy.RequireRole("Administrador", "Operador"));
    options.AddPolicy("RequireAnalistaRole", policy => policy.RequireRole("Administrador", "Operador", "Analista"));
});

// Configuracion EF Core Data Services (DbContext, Repositories, UnitOfWork)
builder.Services.AddDataServices(builder.Configuration);

// Configure Business Services Layer
builder.Services.AddServiceLayer();

// Configure Typed HttpClient for SrtNotificationClient
builder.Services.AddHttpClient<ISrtNotificationClient, SrtNotificationClient>(client =>
{
    var baseUrl = builder.Configuration["SrtMock:BaseUrl"] ?? "http://localhost:8082/";
    if (!baseUrl.EndsWith("/")) baseUrl += "/";
    client.BaseAddress = new Uri(baseUrl);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MiniSiniestros API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT obtenido del endpoint POST /api/auth/login con el formato: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Enable Global Exception Handler
app.UseExceptionHandler();

// Enable Serilog HTTP Logging
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
