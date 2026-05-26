namespace PopayanFilms.Core.Modules.Reservas.Domain;

public class Reserva
{
    public int Id { get; set; }
    public int IdHorario { get; set; }
    public int IdAsiento { get; set; }
    public int IdListaPrecio { get; set; }
    public string FechaReserva { get; set; } = string.Empty;
    public string Estado { get; set; } = "pendiente";
}