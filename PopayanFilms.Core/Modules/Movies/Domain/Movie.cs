namespace PopayanFilms.Core.Modules.Movies.Domain;

public class Movie
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int DuracionMin { get; set; }
    public string Clasificacion { get; set; } = string.Empty;
    public string? FechaEstreno { get; set; }
    public string Estado { get; set; } = "activa";
}