using System.Text;
using HospitalCitas;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

try
{
    Hospital.Iniciar();

    bool salir = false;
    while (!salir)
    {
        switch (Hospital.SeleccionarOpcion())
        {
            case 1: Hospital.RegistrarCitas(); break;
            case 2: Hospital.BuscarPorCodigo(); break;
            case 3: Hospital.ListarCitas(); break;
            case 4: Hospital.BorrarCitas(); break;
            case 5: salir = true; break;
        }
    }
}
catch (EndOfStreamException)
{
    Hospital.Error("Se cerró la entrada de datos. El programa terminará.");
}
catch (Exception ex)
{
    Hospital.Error($"Ocurrió un problema inesperado: {ex.Message}");
}

Console.WriteLine("\n============== FIN DEL SISTEMA ================");