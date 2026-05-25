using PopayanFilms.Core.Modules.Horarios.Domain;
using PopayanFilms.Core.Modules.Horarios.Application;

namespace PopayanFilms.Api.Endpoints;

public static class HorarioEndpoints
{
    public static void MapHorarioEndpoints(this WebApplication app)
    {
        var horarios = app.MapGroup("/horarios").WithTags("Horarios");

        // GET all
        horarios.MapGet("/", async (IHorarioRepository repo) => Results.Ok(await repo.GetAllAsync()));

        // GET by id
        horarios.MapGet("/{id:int}", async (int id, IHorarioRepository repo) =>
        {
            var horario = await repo.GetByIdAsync(id);
            return horario is null ? Results.NotFound() : Results.Ok(horario);
        });

        // GET by pelicula
        horarios.MapGet("/pelicula/{idPelicula:int}", async (int idPelicula, IHorarioRepository repo) =>
            Results.Ok(await repo.GetByPeliculaAsync(idPelicula)));

        // GET by sala
        horarios.MapGet("/sala/{idSala:int}", async (int idSala, IHorarioRepository repo) =>
            Results.Ok(await repo.GetBySalaAsync(idSala)));

        // POST
        horarios.MapPost("/", async (Horario horario, IHorarioRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(horario.FechaHora))
                return Results.BadRequest(new { error = "La fecha y hora es requerida." });
            if (horario.IdSala <= 0)
                return Results.BadRequest(new { error = "La sala es requerida." });
            if (horario.IdPelicula <= 0)
                return Results.BadRequest(new { error = "La película es requerida." });

            horario.Estado = string.IsNullOrWhiteSpace(horario.Estado) ? "activo" : horario.Estado;
            await repo.AddAsync(horario);
            return Results.Created($"/horarios", horario);
        });

        // PUT
        horarios.MapPut("/{id:int}", async (int id, Horario horario, IHorarioRepository repo) =>
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();
            if (string.IsNullOrWhiteSpace(horario.FechaHora))
                return Results.BadRequest(new { error = "La fecha y hora es requerida." });
            if (horario.IdSala <= 0)
                return Results.BadRequest(new { error = "La sala es requerida." });
            if (horario.IdPelicula <= 0)
                return Results.BadRequest(new { error = "La película es requerida." });

            horario.Id = id;
            var rows = await repo.UpdateAsync(horario);
            if (rows == 0) return Results.NotFound();
            return Results.Ok(horario);
        });

        // DELETE
        horarios.MapDelete("/{id:int}", async (int id, IHorarioRepository repo) =>
        {
            var rows = await repo.DeleteAsync(id);
            return rows > 0 ? Results.NoContent() : Results.NotFound();
        });
    }
}