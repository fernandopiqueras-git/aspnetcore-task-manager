# ASP.NET Core Task Manager

Aplicación web para gestionar tareas por estado, prioridad y fecha límite.

## Tecnologías

- ASP.NET Core MVC
- Entity Framework Core 8
- SQL Server LocalDB
- C#
- Razor
- xUnit

## Funciones

- Crear, consultar, editar y eliminar tareas
- Confirmación antes de eliminar
- Cambiar el estado de una tarea
- Validar títulos, descripciones y fechas límite
- Filtrar por texto, estado y prioridad
- Persistencia en SQL Server mediante migraciones
- Diseño responsive
- Pruebas unitarias del repositorio, controlador y validaciones

## Ejecución

```bash
dotnet restore
dotnet run
```

## Pruebas

```bash
dotnet test
```
