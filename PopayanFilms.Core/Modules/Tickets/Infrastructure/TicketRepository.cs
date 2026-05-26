using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.Tickets.Application;
using PopayanFilms.Core.Modules.Tickets.Domain;

namespace PopayanFilms.Core.Modules.Tickets.Infrastructure;

public class TicketRepository : ITicketRepository
{
    private readonly IDapperHelper _db;

    public TicketRepository(IDapperHelper db) => _db = db;

    public Task<IEnumerable<Ticket>> GetAllAsync() =>
        _db.QueryAsync<Ticket>("SELECT * FROM Tickets");

    public Task<Ticket?> GetByIdAsync(int id) =>
        _db.QueryFirstOrDefaultAsync<Ticket>("SELECT * FROM Tickets WHERE Id = @Id", new { Id = id });

    public async Task<int> AddAsync(Ticket entity)
    {
        // Generamos código único si no viene
        if (string.IsNullOrWhiteSpace(entity.Codigo))
            entity.Codigo = $"TKT-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        if (string.IsNullOrWhiteSpace(entity.FechaEmision))
            entity.FechaEmision = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        var sql = """
            INSERT INTO Tickets (IdReserva, Codigo, FechaEmision, Estado)
            VALUES (@IdReserva, @Codigo, @FechaEmision, @Estado);
            SELECT last_insert_rowid();
            """;
        var id = await _db.ExecuteScalarAsync<long>(sql, entity);
        entity.Id = (int)id;
        return entity.Id;
    }

    public async Task<int> UpdateAsync(Ticket entity)
    {
        return await _db.ExecuteAsync("""
            UPDATE Tickets
            SET IdReserva = @IdReserva, Codigo = @Codigo,
                FechaEmision = @FechaEmision, Estado = @Estado
            WHERE Id = @Id
            """, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _db.ExecuteAsync("DELETE FROM Tickets WHERE Id = @Id", new { Id = id });
    }

    public Task<Ticket?> GetByCodigoAsync(string codigo) =>
        _db.QueryFirstOrDefaultAsync<Ticket>("SELECT * FROM Tickets WHERE Codigo = @Codigo", new { Codigo = codigo });

    public Task<IEnumerable<Ticket>> GetByReservaAsync(int idReserva) =>
        _db.QueryAsync<Ticket>("SELECT * FROM Tickets WHERE IdReserva = @IdReserva", new { IdReserva = idReserva });
}