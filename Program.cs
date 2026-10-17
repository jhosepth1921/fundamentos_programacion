using System.Text;
using HospitalCitas;

Console.OutputEncoding = Encoding.UTF8;

bool continuar = true;
while (continuar)
{
    int opcion = Hospital.SeleccionarOpcion();

    if (opcion == 1)
    {
        Hospital.BuscarPorCodigo(Hospital.Citas);
    }
    else if (opcion == 2)
    {
        var nuevasCitas = Hospital.GenerarCita();
        Hospital.Citas.AddRange(nuevasCitas);
    }

    Console.Write("\n¿Desea realizar otra actividad en el sistema? (si/no): ");
    continuar = (Console.ReadLine() ?? "").ToLower() == "si";
}

Console.WriteLine("\n============== FIN DEL SISTEMA ================");
