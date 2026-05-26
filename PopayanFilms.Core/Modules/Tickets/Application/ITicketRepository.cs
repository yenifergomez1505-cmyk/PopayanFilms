using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.Tickets.Domain;

namespace PopayanFilms.Core.Modules.Tickets.Application;

public interface ITicketRepository : IGenericRepository<Ticket, int>
{
    Task<Ticket?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<Ticket>> GetByReservaAsync(int idReserva);
}