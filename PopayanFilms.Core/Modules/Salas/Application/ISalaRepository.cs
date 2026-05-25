using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.Salas.Domain;

namespace PopayanFilms.Core.Modules.Salas.Application;

public interface ISalaRepository : IGenericRepository<Sala, int>
{
    Task<IEnumerable<Sala>> GetByTipoAsync(string tipo);
}