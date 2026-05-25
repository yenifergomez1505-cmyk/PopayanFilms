namespace PopayanFilms.Core.Modules.Asientos.Domain;

public class Asiento
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public string Fila { get; set; } = string.Empty;
    public int IdSala { get; set; }
    public string Estado { get; set; } = "disponible";
}