using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.Asientos.Application;
using PopayanFilms.Core.Modules.Asientos.Domain;

namespace PopayanFilms.Core.Modules.Asientos.Infrastructure;

public class AsientoRepository : IAsientoRepository
{
    private readonly IDapperHelper _db;

    public AsientoRepository(IDapperHelper db) => _db = db;

    public Task<IEnumerable<Asiento>> GetAllAsync() =>
        _db.QueryAsync<Asiento>("SELECT * FROM Asientos");

    public Task<Asiento?> GetByIdAsync(int id) =>
        _db.QueryFirstOrDefaultAsync<Asiento>("SELECT * FROM Asientos WHERE Id = @Id", new { Id = id });

    public async Task<int> AddAsync(Asiento entity)
    {
        var sql = """
            INSERT INTO Asientos (Numero, Fila, IdSala, Estado)
            VALUES (@Numero, @Fila, @IdSala, @Estado);
            SELECT last_insert_rowid();
            """;
        var id = await _db.ExecuteScalarAsync<long>(sql, entity);
        entity.Id = (int)id;
        return entity.Id;
    }

    public async Task<int> UpdateAsync(Asiento entity)
    {
        return await _db.ExecuteAsync("""
            UPDATE Asientos
            SET Numero = @Numero, Fila = @Fila,
                IdSala = @IdSala, Estado = @Estado
            WHERE Id = @Id
            """, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _db.ExecuteAsync("DELETE FROM Asientos WHERE Id = @Id", new { Id = id });
    }

    public Task<IEnumerable<Asiento>> GetBySalaAsync(int idSala) =>
        _db.QueryAsync<Asiento>("SELECT * FROM Asientos WHERE IdSala = @IdSala", new { IdSala = idSala });

    public Task<IEnumerable<Asiento>> GetDisponiblesAsync(int idSala) =>
        _db.QueryAsync<Asiento>("SELECT * FROM Asientos WHERE IdSala = @IdSala AND Estado = 'disponible'", new { IdSala = idSala });
}