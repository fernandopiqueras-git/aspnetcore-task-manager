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

- Crear y eliminar tareas
- Cambiar el estado de una tarea
- Filtrar por texto, estado y prioridad
- Persistencia en SQL Server
- Migraciones de Entity Framework Core
- Diseño responsive
- Pruebas unitarias del repositorio y el controlador

## Ejecución

```bash
dotnet restore
dotnet run
```

La aplicación crea y actualiza la base de datos local mediante migraciones al arrancar.

## Pruebas

```bash
dotnet test
```
