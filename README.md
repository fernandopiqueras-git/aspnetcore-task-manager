# ASP.NET Core Task Manager

Aplicación web para gestionar y organizar tareas.

## Tecnologías

- ASP.NET Core MVC
- Entity Framework Core 8
- SQL Server LocalDB
- C#
- Razor
- FullCalendar 6.1.19
- xUnit

## Funciones

- CRUD completo de tareas
- Estados, prioridades y fechas límite
- Proyectos y etiquetas
- Asignación de responsables
- Comentarios en las tareas
- Búsqueda y filtros
- API JSON para consultar y modificar la planificación de tareas
- Calendario mensual y semanal
- Creación, edición, arrastre y redimensionado de tareas en el calendario
- Lista editable sincronizada con calendario y tablero
- Persistencia mediante migraciones
- Diseño responsive
- Pruebas unitarias

## Vistas

- /Tasks muestra el tablero y el CRUD completo.
- /TaskList muestra todas las tareas y permite editar su planificación en línea.
- /Calendar muestra el calendario mensual y semanal con FullCalendar 6.1.19 desde jsDelivr.

Las tres vistas utilizan los mismos datos persistidos. La lista y el calendario guardan mediante la API y reflejan los cambios al volver a cargarse.

## API de planificación

- GET /api/tasks?start=2026-09-15T00:00:00&end=2026-09-16T00:00:00
- GET /api/tasks/list
- GET /api/tasks/{id}
- POST /api/tasks
- PUT /api/tasks/{id}

Las operaciones de escritura requieren un token antiforgery en la cabecera X-CSRF-TOKEN.

## Ejecución

    dotnet restore
    dotnet run

## Pruebas

    dotnet test
