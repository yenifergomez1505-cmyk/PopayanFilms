using Core.Shared.Interfaces;
using Core.Shared.Infrastructure;
using PopayanFilms.Core.Modules.Movies.Application;
using PopayanFilms.Core.Modules.Movies.Infrastructure;
using PopayanFilms.Core.Modules.Salas.Application;
using PopayanFilms.Core.Modules.Salas.Infrastructure;
using PopayanFilms.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=popayanfilms.db";

builder.Services.AddSingleton<IDapperHelper>(_ => new DapperHelper(conn));
builder.Services.AddTransient<IMovieRepository, MovieRepository>();
builder.Services.AddTransient<ISalaRepository, SalaRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// --- Crear tablas si no existen ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IDapperHelper>();
    
    await db.ExecuteAsync("""
        CREATE TABLE IF NOT EXISTS Movies (
            Id            INTEGER PRIMARY KEY AUTOINCREMENT,
            Titulo        TEXT    NOT NULL,
            Genero        TEXT    NOT NULL,
            DuracionMin   INTEGER NOT NULL,
            Clasificacion TEXT    NOT NULL,
            FechaEstreno  TEXT    NULL,
            Estado        TEXT    NOT NULL DEFAULT 'activa'
        );
        """);

    await db.ExecuteAsync("""
        CREATE TABLE IF NOT EXISTS Salas (
            Id        INTEGER PRIMARY KEY AUTOINCREMENT,
            Nombre    TEXT    NOT NULL,
            Capacidad INTEGER NOT NULL,
            Tipo      TEXT    NOT NULL
        );
        """);
}

// --- Endpoints ---
app.MapMovieEndpoints();
app.MapSalaEndpoints();

app.Run();