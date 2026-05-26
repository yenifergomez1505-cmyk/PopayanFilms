using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.Reservas.Application;
using PopayanFilms.Core.Modules.Reservas.Domain;

namespace PopayanFilms.Core.Modules.Reservas.Infrastructure;

public class ReservaRepository : IReservaRepository
{
    private readonly IDapperHelper _db;

    public ReservaRepository(IDapperHelper db) => _db = db;

    public Task<IEnumerable<Reserva>> GetAllAsync() =>
        _db.QueryAsync<Reserva>("SELECT * FROM Reservas");

    public Task<Reserva?> GetByIdAsync(int id) =>
        _db.QueryFirstOrDefaultAsync<Reserva>("SELECT * FROM Reservas WHERE Id = @Id", new { Id = id });

    public async Task<int> AddAsync(Reserva entity)
    {
        var sql = """
            INSERT INTO Reservas (IdHorario, IdAsiento, IdListaPrecio, FechaReserva, Estado)
            VALUES (@IdHorario, @IdAsiento, @IdListaPrecio, @FechaReserva, @Estado);
            SELECT last_insert_rowid();
            """;
        var id = await _db.ExecuteScalarAsync<long>(sql, entity);
        entity.Id = (int)id;
        return entity.Id;
    }

    public async Task<int> UpdateAsync(Reserva entity)
    {
        return await _db.ExecuteAsync("""
            UPDATE Reservas
            SET IdHorario = @IdHorario, IdAsiento = @IdAsiento,
                IdListaPrecio = @IdListaPrecio, FechaReserva = @FechaReserva,
                Estado = @Estado
            WHERE Id = @Id
            """, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _db.ExecuteAsync("DELETE FROM Reservas WHERE Id = @Id", new { Id = id });
    }

    public Task<IEnumerable<Reserva>> GetByHorarioAsync(int idHorario) =>
        _db.QueryAsync<Reserva>("SELECT * FROM Reservas WHERE IdHorario = @IdHorario", new { IdHorario = idHorario });

    public Task<IEnumerable<Reserva>> GetByEstadoAsync(string estado) =>
        _db.QueryAsync<Reserva>("SELECT * FROM Reservas WHERE Estado = @Estado", new { Estado = estado });
}