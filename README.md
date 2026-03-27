# Notas API 📝

API REST simple construida con ASP.NET Core para gestionar notas en memoria.

## Tecnologías
- ASP.NET Core 10
- C#
- .NET 10

## Endpoints

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | /api/notas | Obtener todas las notas |
| POST | /api/notas | Crear una nota nueva |
| DELETE | /api/notas/{index} | Eliminar una nota por índice |

## Cómo correr el proyecto

1. Clonar el repositorio
2. Entrar a la carpeta del proyecto
3. Ejecutar el siguiente comando:
```bash
dotnet run
```

## Cómo probar los endpoints

### Crear una nota
- Método: POST
- URL: http://localhost:5016/api/notas
- Params: nota = "Tu nota aquí"

### Ver todas las notas
- Método: GET
- URL: http://localhost:5016/api/notas

### Eliminar una nota
- Método: DELETE
- URL: http://localhost:5016/api/notas/{index}