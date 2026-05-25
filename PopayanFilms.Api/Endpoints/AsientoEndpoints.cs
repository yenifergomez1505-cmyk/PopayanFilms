using PopayanFilms.Core.Modules.Asientos.Domain;
using PopayanFilms.Core.Modules.Asientos.Application;

namespace PopayanFilms.Api.Endpoints;

public static class AsientoEndpoints
{
    public static void MapAsientoEndpoints(this WebApplication app)
    {
        var asientos = app.MapGroup("/asientos").WithTags("Asientos");

        // GET all
        asientos.MapGet("/", async (IAsientoRepository repo) => Results.Ok(await repo.GetAllAsync()));

        // GET by id
        asientos.MapGet("/{id:int}", async (int id, IAsientoRepository repo) =>
        {
            var asiento = await repo.GetByIdAsync(id);
            return asiento is null ? Results.NotFound() : Results.Ok(asiento);
        });

        // GET by sala
        asientos.MapGet("/sala/{idSala:int}", async (int idSala, IAsientoRepository repo) =>
            Results.Ok(await repo.GetBySalaAsync(idSala)));

        // GET disponibles by sala
        asientos.MapGet("/sala/{idSala:int}/disponibles", async (int idSala, IAsientoRepository repo) =>
            Results.Ok(await repo.GetDisponiblesAsync(idSala)));

        // POST
        asientos.MapPost("/", async (Asiento asiento, IAsientoRepository repo) =>
        {
            if (asiento.Numero <= 0)
                return Results.BadRequest(new { error = "El número de asiento debe ser mayor a 0." });
            if (string.IsNullOrWhiteSpace(asiento.Fila))
                return Results.BadRequest(new { error = "La fila es requerida." });
            if (asiento.IdSala <= 0)
                return Results.BadRequest(new { error = "La sala es requerida." });

            asiento.Estado = string.IsNullOrWhiteSpace(asiento.Estado) ? "disponible" : asiento.Estado;
            await repo.AddAsync(asiento);
            return Results.Created($"/asientos", asiento);
        });

        // PUT
        asientos.MapPut("/{id:int}", async (int id, Asiento asiento, IAsientoRepository repo) =>
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();
            if (asiento.Numero <= 0)
                return Results.BadRequest(new { error = "El número de asiento debe ser mayor a 0." });
            if (string.IsNullOrWhiteSpace(asiento.Fila))
                return Results.BadRequest(new { error = "La fila es requerida." });
            if (asiento.IdSala <= 0)
                return Results.BadRequest(new { error = "La sala es requerida." });

            asiento.Id = id;
            var rows = await repo.UpdateAsync(asiento);
            if (rows == 0) return Results.NotFound();
            return Results.Ok(asiento);
        });

        // DELETE
        asientos.MapDelete("/{id:int}", async (int id, IAsientoRepository repo) =>
        {
            var rows = await repo.DeleteAsync(id);
            return rows > 0 ? Results.NoContent() : Results.NotFound();
        });
    }
}