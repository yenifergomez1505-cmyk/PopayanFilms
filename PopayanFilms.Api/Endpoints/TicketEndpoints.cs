using PopayanFilms.Core.Modules.Tickets.Domain;
using PopayanFilms.Core.Modules.Tickets.Application;

namespace PopayanFilms.Api.Endpoints;

public static class TicketEndpoints
{
    public static void MapTicketEndpoints(this WebApplication app)
    {
        var tickets = app.MapGroup("/tickets").WithTags("Tickets");

        // GET all
        tickets.MapGet("/", async (ITicketRepository repo) => Results.Ok(await repo.GetAllAsync()));

        // GET by id
        tickets.MapGet("/{id:int}", async (int id, ITicketRepository repo) =>
        {
            var ticket = await repo.GetByIdAsync(id);
            return ticket is null ? Results.NotFound() : Results.Ok(ticket);
        });

        // GET by codigo
        tickets.MapGet("/codigo/{codigo}", async (string codigo, ITicketRepository repo) =>
        {
            var ticket = await repo.GetByCodigoAsync(codigo);
            return ticket is null ? Results.NotFound() : Results.Ok(ticket);
        });

        // GET by reserva
        tickets.MapGet("/reserva/{idReserva:int}", async (int idReserva, ITicketRepository repo) =>
            Results.Ok(await repo.GetByReservaAsync(idReserva)));

        // POST
        tickets.MapPost("/", async (Ticket ticket, ITicketRepository repo) =>
        {
            if (ticket.IdReserva <= 0)
                return Results.BadRequest(new { error = "La reserva es requerida." });

            await repo.AddAsync(ticket);
            return Results.Created($"/tickets", ticket);
        });

        // PUT
        tickets.MapPut("/{id:int}", async (int id, Ticket ticket, ITicketRepository repo) =>
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();
            if (ticket.IdReserva <= 0)
                return Results.BadRequest(new { error = "La reserva es requerida." });

            ticket.Id = id;
            var rows = await repo.UpdateAsync(ticket);
            if (rows == 0) return Results.NotFound();
            return Results.Ok(ticket);
        });

        // DELETE
        tickets.MapDelete("/{id:int}", async (int id, ITicketRepository repo) =>
        {
            var rows = await repo.DeleteAsync(id);
            return rows > 0 ? Results.NoContent() : Results.NotFound();
        });
    }
}