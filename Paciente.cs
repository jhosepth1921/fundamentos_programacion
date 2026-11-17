using System.Globalization;

namespace HospitalCitas;

public class Paciente
{
    public string Nombre { get; }
    public string Apellido { get; }
    public string Dni { get; }
    public string Telefono { get; }
    public string Seguro { get; }

    public string NombreCompleto => $"{Apellido}, {Nombre}";

    public Paciente(string nombre, string apellido, string dni, string telefono, string seguro)
    {
        Nombre = nombre;
        Apellido = apellido;
        Dni = dni;
        Telefono = telefono;
        Seguro = seguro;
    }

    // ---------- REGLAS DE VALIDACIÓN (las usan Hospital y Cita) ----------

    public static bool SoloDigitos(string texto, int largo)
    {
        return texto.Length == largo && texto.All(c => c is >= '0' and <= '9');
    }

    // Quita espacios repetidos y pone formato "Juan Perez"
    public static string NormalizarNombre(string texto)
    {
        string limpio = string.Join(' ', texto.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(limpio.ToLowerInvariant());
    }

    // Solo letras y espacios, entre 2 y 30 caracteres
    public static bool NombreValido(string texto)
    {
        return texto.Length >= 2 && texto.Length <= 30 && texto.All(c => char.IsLetter(c) || c == ' ');
    }

    // DNI peruano: 8 dígitos
    public static bool DniValido(string texto) => SoloDigitos(texto, 8);

    // Celular peruano: 9 dígitos y empieza con 9
    public static bool TelefonoValido(string texto) => SoloDigitos(texto, 9) && texto[0] == '9';
}