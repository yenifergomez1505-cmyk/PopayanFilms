namespace PopayanFilms.Core.Modules.Horarios.Domain;

public class Horario
{
    public int Id { get; set; }
    public string FechaHora { get; set; } = string.Empty;
    public int IdSala { get; set; }
    public int IdPelicula { get; set; }
    public string Estado { get; set; } = "activo";
}