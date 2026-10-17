namespace HospitalCitas;

public class Paciente
{
    public string Codigo { get; }
    public string Nombre { get; }
    public int Edad { get; }

    public Paciente(string codigo, string nombre, int edad)
    {
        Codigo = codigo;
        Nombre = nombre;
        Edad = edad;
    }
}
