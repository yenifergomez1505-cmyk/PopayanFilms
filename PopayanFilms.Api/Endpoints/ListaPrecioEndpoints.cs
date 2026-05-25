using PopayanFilms.Core.Modules.ListaPrecios.Domain;
using PopayanFilms.Core.Modules.ListaPrecios.Application;

namespace PopayanFilms.Api.Endpoints;

public static class ListaPrecioEndpoints
{
    public static void MapListaPrecioEndpoints(this WebApplication app)
    {
        var precios = app.MapGroup("/listaprecios").WithTags("ListaPrecios");

        // GET all
        precios.MapGet("/", async (IListaPrecioRepository repo) => Results.Ok(await repo.GetAllAsync()));

        // GET by id
        precios.MapGet("/{id:int}", async (int id, IListaPrecioRepository repo) =>
        {
            var precio = await repo.GetByIdAsync(id);
            return precio is null ? Results.NotFound() : Results.Ok(precio);
        });

        // GET by tipo
        precios.MapGet("/tipo/{tipo}", async (string tipo, IListaPrecioRepository repo) =>
            Results.Ok(await repo.GetByTipoAsync(tipo)));

        // POST
        precios.MapPost("/", async (ListaPrecio precio, IListaPrecioRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(precio.Descripcion))
                return Results.BadRequest(new { error = "La descripción es requerida." });
            if (precio.Precio <= 0)
                return Results.BadRequest(new { error = "El precio debe ser mayor a 0." });
            if (string.IsNullOrWhiteSpace(precio.Tipo))
                return Results.BadRequest(new { error = "El tipo es requerido." });

            await repo.AddAsync(precio);
            return Results.Created($"/listaprecios", precio);
        });

        // PUT
        precios.MapPut("/{id:int}", async (int id, ListaPrecio precio, IListaPrecioRepository repo) =>
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();
            if (string.IsNullOrWhiteSpace(precio.Descripcion))
                return Results.BadRequest(new { error = "La descripción es requerida." });
            if (precio.Precio <= 0)
                return Results.BadRequest(new { error = "El precio debe ser mayor a 0." });
            if (string.IsNullOrWhiteSpace(precio.Tipo))
                return Results.BadRequest(new { error = "El tipo es requerido." });

            precio.Id = id;
            var rows = await repo.UpdateAsync(precio);
            if (rows == 0) return Results.NotFound();
            return Results.Ok(precio);
        });

        // DELETE
        precios.MapDelete("/{id:int}", async (int id, IListaPrecioRepository repo) =>
        {
            var rows = await repo.DeleteAsync(id);
            return rows > 0 ? Results.NoContent() : Results.NotFound();
        });
    }
}