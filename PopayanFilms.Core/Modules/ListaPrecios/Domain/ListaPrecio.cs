namespace PopayanFilms.Core.Modules.ListaPrecios.Domain;

public class ListaPrecio
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Tipo { get; set; } = string.Empty;
}