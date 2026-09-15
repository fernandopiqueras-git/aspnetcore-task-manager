# ASP.NET Core Task Manager

Aplicación web para gestionar y organizar tareas.

## Tecnologías

- ASP.NET Core MVC
- Entity Framework Core 8
- SQL Server LocalDB
- C#
- Razor
- xUnit

## Funciones

- CRUD completo de tareas
- Estados, prioridades y fechas límite
- Proyectos y etiquetas
- Asignación de responsables
- Comentarios en las tareas
- Búsqueda y filtros
- API JSON para consultar y modificar la planificación de tareas
- Persistencia mediante migraciones
- Diseño responsive
- Pruebas unitarias

## API de planificación

- `GET /api/tasks?start=2026-09-15T00:00:00&end=2026-09-16T00:00:00`
- `GET /api/tasks/{id}`
- `POST /api/tasks`
- `PUT /api/tasks/{id}`

Las operaciones de escritura requieren un token antiforgery en la cabecera `X-CSRF-TOKEN`.

## Ejecución

```bash
dotnet restore
dotnet run
```

## Pruebas

```bash
dotnet test
```
