using Core.Shared.Interfaces;
using Core.Shared.Infrastructure;
using PopayanFilms.Core.Modules.Movies.Application;
using PopayanFilms.Core.Modules.Movies.Infrastructure;
using PopayanFilms.Core.Modules.Salas.Application;
using PopayanFilms.Core.Modules.Salas.Infrastructure;
using PopayanFilms.Core.Modules.Horarios.Application;
using PopayanFilms.Core.Modules.Horarios.Infrastructure;
using PopayanFilms.Core.Modules.ListaPrecios.Application;
using PopayanFilms.Core.Modules.ListaPrecios.Infrastructure;
using PopayanFilms.Api.Endpoints;
using PopayanFilms.Core.Modules.Asientos.Application;
using PopayanFilms.Core.Modules.Asientos.Infrastructure;
using PopayanFilms.Core.Modules.Reservas.Application;
using PopayanFilms.Core.Modules.Reservas.Infrastructure;
using PopayanFilms.Core.Modules.Tickets.Application;
using PopayanFilms.Core.Modules.Tickets.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=popayanfilms.db";

builder.Services.AddSingleton<IDapperHelper>(_ => new DapperHelper(conn));
builder.Services.AddTransient<IMovieRepository, MovieRepository>();
builder.Services.AddTransient<ISalaRepository, SalaRepository>();
builder.Services.AddTransient<IHorarioRepository, HorarioRepository>();
builder.Services.AddTransient<IListaPrecioRepository, ListaPrecioRepository>();
builder.Services.AddTransient<IAsientoRepository, AsientoRepository>();
builder.Services.AddTransient<IReservaRepository, ReservaRepository>();
builder.Services.AddTransient<ITicketRepository, TicketRepository>();

var app = builder.Build();

app.UseCors();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();

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

    await db.ExecuteAsync("""
        CREATE TABLE IF NOT EXISTS Horarios (
            Id          INTEGER PRIMARY KEY AUTOINCREMENT,
            FechaHora   TEXT    NOT NULL,
            IdSala      INTEGER NOT NULL,
            IdPelicula  INTEGER NOT NULL,
            Estado      TEXT    NOT NULL DEFAULT 'activo'
        );
        """);

    await db.ExecuteAsync("""
        CREATE TABLE IF NOT EXISTS ListaPrecios (
            Id          INTEGER PRIMARY KEY AUTOINCREMENT,
            Descripcion TEXT    NOT NULL,
            Precio      REAL    NOT NULL,
            Tipo        TEXT    NOT NULL
        );
        """);

        await db.ExecuteAsync("""
    CREATE TABLE IF NOT EXISTS Asientos (
        Id      INTEGER PRIMARY KEY AUTOINCREMENT,
        Numero  INTEGER NOT NULL,
        Fila    TEXT    NOT NULL,
        IdSala  INTEGER NOT NULL,
        Estado  TEXT    NOT NULL DEFAULT 'disponible'
    );
    """);

    await db.ExecuteAsync("""
    CREATE TABLE IF NOT EXISTS Reservas (
        Id            INTEGER PRIMARY KEY AUTOINCREMENT,
        IdHorario     INTEGER NOT NULL,
        IdAsiento     INTEGER NOT NULL,
        IdListaPrecio INTEGER NOT NULL,
        FechaReserva  TEXT    NOT NULL,
        Estado        TEXT    NOT NULL DEFAULT 'pendiente'
    );
    """);

    await db.ExecuteAsync("""
    CREATE TABLE IF NOT EXISTS Tickets (
        Id           INTEGER PRIMARY KEY AUTOINCREMENT,
        IdReserva    INTEGER NOT NULL,
        Codigo       TEXT    NOT NULL UNIQUE,
        FechaEmision TEXT    NOT NULL,
        Estado       TEXT    NOT NULL DEFAULT 'activo'
    );
    """);
}

app.MapMovieEndpoints();
app.MapSalaEndpoints();
app.MapHorarioEndpoints();
app.MapListaPrecioEndpoints();
app.MapAsientoEndpoints();
app.MapReservaEndpoints();
app.MapTicketEndpoints();
app.Run();