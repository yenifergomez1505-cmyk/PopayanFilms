using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.Salas.Application;
using PopayanFilms.Core.Modules.Salas.Domain;

namespace PopayanFilms.Core.Modules.Salas.Infrastructure;

public class SalaRepository : ISalaRepository
{
    private readonly IDapperHelper _db;

    public SalaRepository(IDapperHelper db) => _db = db;

    public Task<IEnumerable<Sala>> GetAllAsync() =>
        _db.QueryAsync<Sala>("SELECT * FROM Salas");

    public Task<Sala?> GetByIdAsync(int id) =>
        _db.QueryFirstOrDefaultAsync<Sala>("SELECT * FROM Salas WHERE Id = @Id", new { Id = id });

    public async Task<int> AddAsync(Sala entity)
    {
        var sql = """
            INSERT INTO Salas (Nombre, Capacidad, Tipo)
            VALUES (@Nombre, @Capacidad, @Tipo);
            SELECT last_insert_rowid();
            """;
        var id = await _db.ExecuteScalarAsync<long>(sql, entity);
        entity.Id = (int)id;
        return entity.Id;
    }

    public async Task<int> UpdateAsync(Sala entity)
    {
        return await _db.ExecuteAsync("""
            UPDATE Salas
            SET Nombre = @Nombre, Capacidad = @Capacidad, Tipo = @Tipo
            WHERE Id = @Id
            """, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _db.ExecuteAsync("DELETE FROM Salas WHERE Id = @Id", new { Id = id });
    }

    public Task<IEnumerable<Sala>> GetByTipoAsync(string tipo) =>
        _db.QueryAsync<Sala>("SELECT * FROM Salas WHERE Tipo = @Tipo", new { Tipo = tipo });
}