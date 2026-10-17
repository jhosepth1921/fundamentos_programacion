namespace HospitalCitas;

public class Cita
{
    public string Codigo { get; }
    public Paciente Paciente { get; }
    public Medico Medico { get; }
    public string Fecha { get; }

    public Cita(string codigo, Paciente paciente, Medico medico, string fecha)
    {
        Codigo = codigo;
        Paciente = paciente;
        Medico = medico;
        Fecha = fecha;
    }

    public string Resumen()
    {
        return $"Código de cita: CIT-{Codigo}\n" +
               $"Código del paciente: PAC-{Paciente.Codigo}\n" +
               $"Nombre del paciente: {Paciente.Nombre}\n" +
               $"Edad del paciente: {Paciente.Edad}\n" +
               $"Código del médico: {Medico.Codigo}\n" +
               $"Médico/a: Dr. {Medico.Nombre}\n" +
               $"Área de Especialidad: {Medico.Especialidad}\n" +
               $"Fecha seleccionada para la cita: {Fecha}";
    }
}
