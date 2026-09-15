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
- Persistencia mediante migraciones
- Diseño responsive
- Pruebas unitarias

## Calendario

La vista /Calendar utiliza FullCalendar 6.1.19 desde jsDelivr. Permite cambiar entre mes y semana, crear una tarea pulsando una fecha, editarla al pulsar sobre ella y modificar sus fechas mediante arrastre o redimensionado.

## API de planificación

- GET /api/tasks?start=2026-09-15T00:00:00&end=2026-09-16T00:00:00
- GET /api/tasks/{id}
- POST /api/tasks
- PUT /api/tasks/{id}

Las operaciones de escritura requieren un token antiforgery en la cabecera X-CSRF-TOKEN.

## Ejecución

    dotnet restore
    dotnet run

## Pruebas

    dotnet test
