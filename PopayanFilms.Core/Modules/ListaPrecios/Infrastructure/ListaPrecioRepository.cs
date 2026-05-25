using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.ListaPrecios.Application;
using PopayanFilms.Core.Modules.ListaPrecios.Domain;

namespace PopayanFilms.Core.Modules.ListaPrecios.Infrastructure;

public class ListaPrecioRepository : IListaPrecioRepository
{
    private readonly IDapperHelper _db;

    public ListaPrecioRepository(IDapperHelper db) => _db = db;

    public Task<IEnumerable<ListaPrecio>> GetAllAsync() =>
        _db.QueryAsync<ListaPrecio>("SELECT * FROM ListaPrecios");

    public Task<ListaPrecio?> GetByIdAsync(int id) =>
        _db.QueryFirstOrDefaultAsync<ListaPrecio>("SELECT * FROM ListaPrecios WHERE Id = @Id", new { Id = id });

    public async Task<int> AddAsync(ListaPrecio entity)
    {
        var sql = """
            INSERT INTO ListaPrecios (Descripcion, Precio, Tipo)
            VALUES (@Descripcion, @Precio, @Tipo);
            SELECT last_insert_rowid();
            """;
        var id = await _db.ExecuteScalarAsync<long>(sql, entity);
        entity.Id = (int)id;
        return entity.Id;
    }

    public async Task<int> UpdateAsync(ListaPrecio entity)
    {
        return await _db.ExecuteAsync("""
            UPDATE ListaPrecios
            SET Descripcion = @Descripcion, Precio = @Precio, Tipo = @Tipo
            WHERE Id = @Id
            """, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _db.ExecuteAsync("DELETE FROM ListaPrecios WHERE Id = @Id", new { Id = id });
    }

    public Task<IEnumerable<ListaPrecio>> GetByTipoAsync(string tipo) =>
        _db.QueryAsync<ListaPrecio>("SELECT * FROM ListaPrecios WHERE Tipo = @Tipo", new { Tipo = tipo });
}