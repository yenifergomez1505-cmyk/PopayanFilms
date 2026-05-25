using PopayanFilms.Core.Modules.Movies.Domain;
using PopayanFilms.Core.Modules.Movies.Application;

namespace PopayanFilms.Api.Endpoints;

public static class MovieEndpoints
{
    public static void MapMovieEndpoints(this WebApplication app)
    {
        var movies = app.MapGroup("/movies").WithTags("Movies");

        // GET all movies
        movies.MapGet("/", async (IMovieRepository repo) => Results.Ok(await repo.GetAllAsync()));

        // GET movie by id
        movies.MapGet("/{id:int}", async (int id, IMovieRepository repo) =>
        {
            var movie = await repo.GetByIdAsync(id);
            return movie is null ? Results.NotFound() : Results.Ok(movie);
        });

        // POST create movie
        movies.MapPost("/", async (Movie movie, IMovieRepository repo) =>
        {
            if (string.IsNullOrWhiteSpace(movie.Titulo) || movie.Titulo.Length > 200)
                return Results.BadRequest(new { error = "El título es requerido y debe tener máximo 200 caracteres." });
            if (string.IsNullOrWhiteSpace(movie.Genero))
                return Results.BadRequest(new { error = "El género es requerido." });
            if (movie.DuracionMin <= 0)
                return Results.BadRequest(new { error = "La duración debe ser mayor a 0 minutos." });
            if (string.IsNullOrWhiteSpace(movie.Clasificacion))
                return Results.BadRequest(new { error = "La clasificación es requerida." });

            movie.Estado = string.IsNullOrWhiteSpace(movie.Estado) ? "activa" : movie.Estado;
            await repo.AddAsync(movie);
            return Results.Created($"/movies", movie);
        });

        // PUT update movie
        movies.MapPut("/{id:int}", async (int id, Movie movie, IMovieRepository repo) =>
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing is null) return Results.NotFound();
            if (string.IsNullOrWhiteSpace(movie.Titulo) || movie.Titulo.Length > 200)
                return Results.BadRequest(new { error = "El título es requerido y debe tener máximo 200 caracteres." });
            if (string.IsNullOrWhiteSpace(movie.Genero))
                return Results.BadRequest(new { error = "El género es requerido." });
            if (movie.DuracionMin <= 0)
                return Results.BadRequest(new { error = "La duración debe ser mayor a 0 minutos." });
            if (string.IsNullOrWhiteSpace(movie.Clasificacion))
                return Results.BadRequest(new { error = "La clasificación es requerida." });

            movie.Id = id;
            var rows = await repo.UpdateAsync(movie);
            if (rows == 0) return Results.NotFound();
            return Results.Ok(movie);
        });

        // DELETE movie
        movies.MapDelete("/{id:int}", async (int id, IMovieRepository repo) =>
        {
            var rows = await repo.DeleteAsync(id);
            return rows > 0 ? Results.NoContent() : Results.NotFound();
        });
    }
}