namespace NotasNzx.Modelos;

public class Nota
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Contenido { get; set; } = string.Empty;
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    // Relación con el usuario dueño de la nota
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    // Relación opcional con carpeta
    public Guid? CarpetaId { get; set; }
    public Carpeta? Carpeta { get; set; }

    //Titulo
    public string Titulo { get; set; } = "Nueva nota";
}