using System.Globalization;

namespace HospitalCitas;

public class Cita
{
    public const string FormatoFechaHora = "dd/MM/yyyy HH:mm";

    public int Numero { get; }
    public Paciente Paciente { get; }
    public Medico Medico { get; }
    public DateTime FechaHora { get; }
    public int Turno { get; }

    public string Codigo => $"CIT-{Numero:0000}";
    public string Area => Medico.Especialidad;

    public Cita(int numero, Paciente paciente, Medico medico, DateTime fechaHora, int turno)
    {
        Numero = numero;
        Paciente = paciente;
        Medico = medico;
        FechaHora = fechaHora;
        Turno = turno;
    }

    public string Resumen()
    {
        return $"  Código de cita    : {Codigo}\n" +
               $"  Fecha y hora      : {FechaHora.ToString(FormatoFechaHora, CultureInfo.InvariantCulture)}\n" +
               $"  Nombres           : {Paciente.Nombre}\n" +
               $"  Apellidos         : {Paciente.Apellido}\n" +
               $"  DNI               : {Paciente.Dni}\n" +
               $"  Teléfono          : {Paciente.Telefono}\n" +
               $"  Seguro            : {Paciente.Seguro}\n" +
               $"  Área              : {Area}\n" +
               $"  Médico/a          : Dr(a). {Medico.Nombre} ({Medico.Codigo})\n" +
               $"  Turno en el área  : {Turno}";
    }

    // ---------- ARCHIVO DE TEXTO ----------
    // Formato: Numero|Nombres|Apellidos|DNI|Telefono|Seguro|Area|FechaHora|Turno

    public string ALinea()
    {
        string fecha = FechaHora.ToString(FormatoFechaHora, CultureInfo.InvariantCulture);
        return $"{Numero}|{Paciente.Nombre}|{Paciente.Apellido}|{Paciente.Dni}|{Paciente.Telefono}|{Paciente.Seguro}|{Area}|{fecha}|{Turno}";
    }

    // Convierte una línea del archivo en una Cita. Si algo está mal, devuelve false y explica por qué.
    public static bool TryDesdeLinea(string linea, IReadOnlyList<Medico> medicos, out Cita? cita, out string error)
    {
        cita = null;
        error = "";
        string[] p = linea.Split('|');

        if (p.Length != 9)
        {
            error = $"debe tener 9 campos separados por '|' y tiene {p.Length}.";
            return false;
        }
        if (!int.TryParse(p[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int numero) || numero <= 0)
        {
            error = $"el número '{p[0]}' no es válido.";
            return false;
        }
        string nombre = p[1].Trim();
        string apellido = p[2].Trim();
        if (nombre.Length == 0 || apellido.Length == 0)
        {
            error = "el nombre o el apellido están vacíos.";
            return false;
        }
        if (!Paciente.DniValido(p[3]))
        {
            error = $"el DNI '{p[3]}' debe tener 8 números.";
            return false;
        }
        if (!Paciente.SoloDigitos(p[4], 9))
        {
            error = $"el teléfono '{p[4]}' debe tener 9 números.";
            return false;
        }
        string seguro = p[5].Trim();
        if (seguro.Length == 0)
        {
            error = "el seguro está vacío.";
            return false;
        }
        Medico? medico = medicos.FirstOrDefault(m =>
            string.Equals(m.Especialidad, p[6].Trim(), StringComparison.OrdinalIgnoreCase));
        if (medico is null)
        {
            error = $"el área '{p[6]}' no existe en el hospital.";
            return false;
        }
        if (!DateTime.TryParseExact(p[7], FormatoFechaHora, CultureInfo.InvariantCulture,
                                    DateTimeStyles.None, out DateTime fechaHora))
        {
            error = $"la fecha '{p[7]}' no tiene formato dd/mm/aaaa hh:mm.";
            return false;
        }
        if (!int.TryParse(p[8], NumberStyles.Integer, CultureInfo.InvariantCulture, out int turno) || turno <= 0)
        {
            error = $"el turno '{p[8]}' no es válido.";
            return false;
        }

        var paciente = new Paciente(nombre, apellido, p[3], p[4], seguro);
        cita = new Cita(numero, paciente, medico, fechaHora, turno);
        return true;
    }
}