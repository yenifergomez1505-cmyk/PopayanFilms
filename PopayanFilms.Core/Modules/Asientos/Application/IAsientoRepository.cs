using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.Asientos.Domain;

namespace PopayanFilms.Core.Modules.Asientos.Application;

public interface IAsientoRepository : IGenericRepository<Asiento, int>
{
    Task<IEnumerable<Asiento>> GetBySalaAsync(int idSala);
    Task<IEnumerable<Asiento>> GetDisponiblesAsync(int idSala);
}