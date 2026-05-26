namespace PopayanFilms.Core.Modules.Tickets.Domain;

public class Ticket
{
    public int Id { get; set; }
    public int IdReserva { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string FechaEmision { get; set; } = string.Empty;
    public string Estado { get; set; } = "activo";
}