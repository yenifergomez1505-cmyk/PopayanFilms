namespace PopayanFilms.Core.Modules.Salas.Domain;

public class Sala
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Capacidad { get; set; }
    public string Tipo { get; set; } = string.Empty;
}