using PopayanFilms.Core.Modules.Reservas.Domain;
using PopayanFilms.Core.Modules.Reservas.Application;

namespace PopayanFilms.Api.Endpoints;

public static class ReservaEndpoints
{
    public static void MapReservaEndpoints(this WebApplication app)
    {
        var reservas = app.MapGroup("/reservas").WithTags("Reservas");

        // GET all
        reservas.MapGet("/", async (IReservaRepository repo) => Results.Ok(await repo.GetAllAsync()));

        // GET by id
        reservas.MapGet("/{id:int}", async (int id, IReservaRepository repo) =>
        {
            var reserva = await repo.GetByIdAsync(id);
            return reserva is null ? Results.NotFound() : Results.Ok(reserva);
        });

        // GET by horario
        reservas.MapGet("/horario/{idHorario:int}", async (int idHorario, IReservaRepository repo) =>
            Results.Ok(await repo.GetByHorarioAsync(idHorario)));

        // GET by estado
        reservas.MapGet("/estado/{estado}", async (string estado, IReservaRepository repo) =>
            Results.Ok(await repo.GetByEstadoAsync(estado)));

        // POST
        reservas.MapPost("/", async (Reserva reserva, IReservaRepository repo) =>
        {
            if (reserva.IdHorario <= 0)
                return Results.BadRequest(new { error = "El horario es requerido." });
            if (reserva.IdAsiento <= 0)
                return Results.BadRequest(new { error = "El asiento es requerido." });
            if (reserva.IdListaPrecio <= 0)
                return Results.BadRequest(new { error = "El precio es requerido." });

            if (string.IsNullOrWhiteSpace(reserva.FechaReserva))
                reserva.FechaReserva = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            reserva.Estado = string.IsNullOrWhiteSpace(reserva.Estado) ? "pendiente" : reserva.Estado;

            await repo.AddAsync(reserva);
            return Results.Created($"/reservas", reserva);
        });

        // PUT
        reservas.MapPut("/{id:int}", async (int id, Reserva reserva, IReservaRepository repo) =>
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();
            if (reserva.IdHorario <= 0)
                return Results.BadRequest(new { error = "El horario es requerido." });
            if (reserva.IdAsiento <= 0)
                return Results.BadRequest(new { error = "El asiento es requerido." });
            if (reserva.IdListaPrecio <= 0)
                return Results.BadRequest(new { error = "El precio es requerido." });

            reserva.Id = id;
            var rows = await repo.UpdateAsync(reserva);
            if (rows == 0) return Results.NotFound();
            return Results.Ok(reserva);
        });

        // DELETE
        reservas.MapDelete("/{id:int}", async (int id, IReservaRepository repo) =>
        {
            var rows = await repo.DeleteAsync(id);
            return rows > 0 ? Results.NoContent() : Results.NotFound();
        });
    }
}