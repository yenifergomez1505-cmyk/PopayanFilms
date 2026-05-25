using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.Movies.Application;
using PopayanFilms.Core.Modules.Movies.Domain;

namespace PopayanFilms.Core.Modules.Movies.Infrastructure;

public class MovieRepository : IMovieRepository
{
    private readonly IDapperHelper _db;

    public MovieRepository(IDapperHelper db) => _db = db;

    public Task<IEnumerable<Movie>> GetAllAsync() =>
        _db.QueryAsync<Movie>("SELECT * FROM Movies");

    public Task<Movie?> GetByIdAsync(int id) =>
        _db.QueryFirstOrDefaultAsync<Movie>("SELECT * FROM Movies WHERE Id = @Id", new { Id = id });

    public async Task<int> AddAsync(Movie entity)
    {
        var sql = """
            INSERT INTO Movies (Titulo, Genero, DuracionMin, Clasificacion, FechaEstreno, Estado)
            VALUES (@Titulo, @Genero, @DuracionMin, @Clasificacion, @FechaEstreno, @Estado);
            SELECT last_insert_rowid();
            """;
        var id = await _db.ExecuteScalarAsync<long>(sql, entity);
        entity.Id = (int)id;
        return entity.Id;
    }

    public async Task<int> UpdateAsync(Movie entity)
    {
        return await _db.ExecuteAsync("""
            UPDATE Movies
            SET Titulo = @Titulo, Genero = @Genero, DuracionMin = @DuracionMin,
                Clasificacion = @Clasificacion, FechaEstreno = @FechaEstreno, Estado = @Estado
            WHERE Id = @Id
            """, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _db.ExecuteAsync("DELETE FROM Movies WHERE Id = @Id", new { Id = id });
    }

    public Task<IEnumerable<Movie>> GetActivasAsync() =>
        _db.QueryAsync<Movie>("SELECT * FROM Movies WHERE Estado = 'activa'");
}