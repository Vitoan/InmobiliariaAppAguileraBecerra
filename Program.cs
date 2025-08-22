using System;
using System.Collections.Generic;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositorios;

public class Program
{
    public static void Main(string[] args)
    {
        // Instancia del repositorio de propietarios
        var repositorio = new RepositorioPropietario();

        Console.WriteLine("Probando la conexión y los métodos del repositorio de propietarios...");
        Console.WriteLine("---------------------------------------------------------------------");

        // 1. Probar la inserción (Alta)
        try
        {
            var nuevoPropietario = new Propietario
            {
                Nombre = "Juan",
                Apellido = "Perez",
                DNI = "12345678",
                Telefono = "123-456-7890",
                Email = "juan.perez@example.com",
                Clave = "password123"
            };

            var idPropietario = repositorio.Alta(nuevoPropietario);
            Console.WriteLine($"Propietario insertado con ID: {idPropietario}");

            // 2. Probar la consulta por ID
            var propietarioInsertado = repositorio.ObtenerPorId(idPropietario);
            Console.WriteLine($"Propietario obtenido: {propietarioInsertado.Nombre} {propietarioInsertado.Apellido}");

            // 3. Probar la modificación
            propietarioInsertado.Telefono = "987-654-3210";
            var filasAfectadasModif = repositorio.Modificacion(propietarioInsertado);
            Console.WriteLine($"Propietario modificado. Filas afectadas: {filasAfectadasModif}");

            // 4. Probar la consulta de todos
            var todosLosPropietarios = repositorio.ObtenerTodos();
            Console.WriteLine($"Total de propietarios en la base de datos: {todosLosPropietarios.Count}");

            // 5. Probar la eliminación (Baja)
            var filasAfectadasBaja = repositorio.Baja(idPropietario);
            Console.WriteLine($"Propietario eliminado. Filas afectadas: {filasAfectadasBaja}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocurrió un error al probar la conexión y los métodos del repositorio:");
            Console.WriteLine(ex.Message);
        }

        Console.WriteLine("Prueba finalizada.");
        Console.ReadKey(); // Espera a que el usuario presione una tecla para cerrar la consola
    }
}
