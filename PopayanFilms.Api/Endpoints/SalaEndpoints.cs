using PopayanFilms.Core.Modules.Salas.Domain;
using PopayanFilms.Core.Modules.Salas.Application;

namespace PopayanFilms.Api.Endpoints;

public static class SalaEndpoints
{
    public static void MapSalaEndpoints(this WebApplication app)
    {
        var salas = app.MapGroup("/salas").WithTags("Salas");

        // GET all salas
        salas.MapGet("/", async (ISalaRepository repo) => Results.Ok(await repo.GetAllAsync()));

        // GET sala by id
        salas.MapGet("/{id:int}", async (int id, ISalaRepository repo) =>
        {
            var sala = await repo.GetByIdAsync(id);
            return sala is null ? Results.NotFound() : Results.Ok(sala);
        });

        // GET salas by tipo
        salas.MapGet("/tipo/{tipo}", async (string tipo, ISalaRepository repo) =>
            Results.Ok(await repo.GetByTipoAsync(tipo)));

        // POST create sala
        salas.MapPost("/", async (Sala sala, ISalaRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(sala.Nombre))
                return Results.BadRequest(new { error = "El nombre es requerido." });
            if (sala.Capacidad <= 0)
                return Results.BadRequest(new { error = "La capacidad debe ser mayor a 0." });
            if (string.IsNullOrWhiteSpace(sala.Tipo))
                return Results.BadRequest(new { error = "El tipo es requerido." });

            await repo.AddAsync(sala);
            return Results.Created($"/salas", sala);
        });

        // PUT update sala
        salas.MapPut("/{id:int}", async (int id, Sala sala, ISalaRepository repo) =>
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();
            if (string.IsNullOrWhiteSpace(sala.Nombre))
                return Results.BadRequest(new { error = "El nombre es requerido." });
            if (sala.Capacidad <= 0)
                return Results.BadRequest(new { error = "La capacidad debe ser mayor a 0." });
            if (string.IsNullOrWhiteSpace(sala.Tipo))
                return Results.BadRequest(new { error = "El tipo es requerido." });

            sala.Id = id;
            var rows = await repo.UpdateAsync(sala);
            if (rows == 0) return Results.NotFound();
            return Results.Ok(sala);
        });

        // DELETE sala
        salas.MapDelete("/{id:int}", async (int id, ISalaRepository repo) =>
        {
            var rows = await repo.DeleteAsync(id);
            return rows > 0 ? Results.NoContent() : Results.NotFound();
        });
    }
}