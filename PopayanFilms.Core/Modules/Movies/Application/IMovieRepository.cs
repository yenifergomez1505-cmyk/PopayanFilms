using Core.Shared.Interfaces;
using PopayanFilms.Core.Modules.Movies.Domain;

namespace PopayanFilms.Core.Modules.Movies.Application;

public interface IMovieRepository : IGenericRepository<Movie, int>
{
    Task<IEnumerable<Movie>> GetActivasAsync();
}