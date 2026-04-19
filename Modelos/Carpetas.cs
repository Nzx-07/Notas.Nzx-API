namespace NotasNzx.Modelos;

public class Carpeta
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
}