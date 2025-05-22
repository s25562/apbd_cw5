using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication1.Data;
using WebApplication1.Repositories;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Implementation;
using WebApplication1.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Dodajemy kontrolery do kontenera DI
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Konfiguracja JSON, aby uniknąć cyklicznych referencji
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Konfiguracja Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Prescription API", Version = "v1" });
});

// Konfiguracja bazy danych
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// Rejestracja repozytoriów
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<IMedicamentRepository, MedicamentRepository>();

// Rejestracja serwisów
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
builder.Services.AddScoped<IPatientService, PatientService>();

// Konfiguracja CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Konfiguracja pipeline żądania HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Tworzymy bazę danych i migracje przy starcie aplikacji w środowisku deweloperskim
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        // Opcjonalnie: usuń i utwórz bazę od nowa (do testów)
        // dbContext.Database.EnsureDeleted(); 

        dbContext.Database.Migrate();
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();