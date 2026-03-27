var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var notas = new List<string>();

app.MapGet("/api/notas", () =>
{
    return notas;

});

app.MapPost("/api/notas", (string nota) =>
{
    notas.Add(nota);
    return Results.Created("/api/notas", nota);

});

app.MapDelete("/api/notas/{index}", (int index) =>
{
    if (index < 0 || index >= notas.Count)
    {
        return  Results.NotFound("Nota no encontrada");
    }

    notas.RemoveAt(index);
    return Results.Ok("Nota eliminada");

});

app.Run();