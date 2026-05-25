using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.Horarios.Application;
using PopayanFilms.Core.Modules.Horarios.Domain;

namespace PopayanFilms.Core.Modules.Horarios.Infrastructure;

public class HorarioRepository : IHorarioRepository
{
    private readonly IDapperHelper _db;

    public HorarioRepository(IDapperHelper db) => _db = db;

    public Task<IEnumerable<Horario>> GetAllAsync() =>
        _db.QueryAsync<Horario>("SELECT * FROM Horarios");

    public Task<Horario?> GetByIdAsync(int id) =>
        _db.QueryFirstOrDefaultAsync<Horario>("SELECT * FROM Horarios WHERE Id = @Id", new { Id = id });

    public async Task<int> AddAsync(Horario entity)
    {
        var sql = """
            INSERT INTO Horarios (FechaHora, IdSala, IdPelicula, Estado)
            VALUES (@FechaHora, @IdSala, @IdPelicula, @Estado);
            SELECT last_insert_rowid();
            """;
        var id = await _db.ExecuteScalarAsync<long>(sql, entity);
        entity.Id = (int)id;
        return entity.Id;
    }

    public async Task<int> UpdateAsync(Horario entity)
    {
        return await _db.ExecuteAsync("""
            UPDATE Horarios
            SET FechaHora = @FechaHora, IdSala = @IdSala,
                IdPelicula = @IdPelicula, Estado = @Estado
            WHERE Id = @Id
            """, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _db.ExecuteAsync("DELETE FROM Horarios WHERE Id = @Id", new { Id = id });
    }

    public Task<IEnumerable<Horario>> GetByPeliculaAsync(int idPelicula) =>
        _db.QueryAsync<Horario>("SELECT * FROM Horarios WHERE IdPelicula = @IdPelicula", new { IdPelicula = idPelicula });

    public Task<IEnumerable<Horario>> GetBySalaAsync(int idSala) =>
        _db.QueryAsync<Horario>("SELECT * FROM Horarios WHERE IdSala = @IdSala", new { IdSala = idSala });
}