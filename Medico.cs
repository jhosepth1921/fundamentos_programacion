namespace HospitalCitas;

public class Medico
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string Especialidad { get; }

    public Medico(string codigo, string nombre, string especialidad)
    {
        Codigo = codigo;
        Nombre = nombre;
        Especialidad = especialidad;
    }
}
