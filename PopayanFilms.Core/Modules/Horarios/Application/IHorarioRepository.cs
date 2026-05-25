using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.Horarios.Domain;

namespace PopayanFilms.Core.Modules.Horarios.Application;

public interface IHorarioRepository : IGenericRepository<Horario, int>
{
    Task<IEnumerable<Horario>> GetByPeliculaAsync(int idPelicula);
    Task<IEnumerable<Horario>> GetBySalaAsync(int idSala);
}