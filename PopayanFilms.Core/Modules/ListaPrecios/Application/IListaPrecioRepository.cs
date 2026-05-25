using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.ListaPrecios.Domain;

namespace PopayanFilms.Core.Modules.ListaPrecios.Application;

public interface IListaPrecioRepository : IGenericRepository<ListaPrecio, int>
{
    Task<IEnumerable<ListaPrecio>> GetByTipoAsync(string tipo);
}