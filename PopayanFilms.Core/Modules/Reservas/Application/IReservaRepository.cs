using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.Reservas.Domain;

namespace PopayanFilms.Core.Modules.Reservas.Application;

public interface IReservaRepository : IGenericRepository<Reserva, int>
{
    Task<IEnumerable<Reserva>> GetByHorarioAsync(int idHorario);
    Task<IEnumerable<Reserva>> GetByEstadoAsync(string estado);
}