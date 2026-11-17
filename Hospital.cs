using System.Globalization;
using System.Text;

namespace HospitalCitas;

// Equivale a las funciones y listas que en Python estaban en cita.py
public static class Hospital
{
    public static readonly List<Medico> Medicos = new()
    {
        new Medico("MED-001", "Luis Ramos", "Nutrición"),
        new Medico("MED-002", "Ana Torres", "Psicología"),
        new Medico("MED-003", "Carlos Pérez", "Odontología"),
        new Medico("MED-004", "María Flores", "Cardiología"),
        new Medico("MED-005", "Pedro Sánchez", "Pediatría"),
        new Medico("MED-006", "Rosa Vega", "Medicina General"),
        new Medico("MED-007", "Elena Castro", "Ginecología"),
        new Medico("MED-008", "Jorge Quispe", "Traumatología")
    };

    public static readonly string[] Seguros = { "SIS", "EsSalud", "Sin seguro" };

    public static readonly List<Cita> Citas = new();

    // ==================== ARCHIVO DE TEXTO ====================

    // citas.txt se crea en la carpeta del programa
    private static readonly string Carpeta = ObtenerCarpeta();
    public static readonly string Ruta = Path.Combine(Carpeta, "citas.txt");

    // Si el archivo existe pero no se pudo leer, no se guarda encima para no perder datos
    private static bool _puedeGuardar = true;

    private static string ObtenerCarpeta()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (dir.GetFiles("*.csproj").Length > 0)
                return dir.FullName;
            dir = dir.Parent;
        }
        return AppContext.BaseDirectory;
    }

    public static void Iniciar()
    {
        Citas.Clear();
        Citas.AddRange(Cargar());
    }

    private static bool Guardar()
    {
        if (!_puedeGuardar)
        {
            Error("No se guarda para no sobrescribir un archivo que no se pudo leer.");
            return false;
        }

        try
        {
            Directory.CreateDirectory(Carpeta);

            var lineas = new List<string>
            {
                "# Citas del hospital. Formato: Numero|Nombres|Apellidos|DNI|Telefono|Seguro|Area|FechaHora|Turno",
                "# Las líneas que empiezan con # se ignoran."
            };
            lineas.AddRange(Citas.Select(c => c.ALinea()));

            // Se escribe en un archivo temporal y luego se reemplaza,
            // así el archivo original nunca queda a medias.
            string temporal = Ruta + ".tmp";
            File.WriteAllLines(temporal, lineas, Encoding.UTF8);
            File.Move(temporal, Ruta, overwrite: true);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Error($"No se pudo guardar el archivo de citas: {ex.Message}");
            return false;
        }
    }

    private static bool Respaldar()
    {
        try
        {
            if (File.Exists(Ruta))
                File.Copy(Ruta, Ruta + ".bak", true);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static List<Cita> Cargar()
    {
        var citas = new List<Cita>();

        // Primera vez: crea el archivo vacío
        if (!File.Exists(Ruta))
        {
            Guardar();
            return citas;
        }

        string[] lineas;
        try
        {
            lineas = File.ReadAllLines(Ruta, Encoding.UTF8);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Error($"No se pudo leer el archivo de citas: {ex.Message}");
            _puedeGuardar = false;
            return citas;
        }

        var errores = new List<string>();

        for (int i = 0; i < lineas.Length; i++)
        {
            string linea = lineas[i].Trim();
            if (linea.Length == 0 || linea.StartsWith('#')) continue;

            int n = i + 1;
            if (!Cita.TryDesdeLinea(linea, Medicos, out Cita? cita, out string motivo) || cita is null)
            {
                errores.Add($"Línea {n}: {motivo}");
                continue;
            }
            if (citas.Any(c => c.Numero == cita.Numero))
            {
                errores.Add($"Línea {n}: el número de cita {cita.Numero} está repetido.");
                continue;
            }
            citas.Add(cita);
        }

        if (errores.Count > 0)
        {
            bool copiado = Respaldar();
            Error("Se encontraron problemas en citas.txt (esas líneas se omitieron):");
            foreach (string e in errores) Console.WriteLine($"     - {e}");

            if (copiado)
                Console.WriteLine($"   Se guardó una copia del archivo original en: {Ruta}.bak");
            else
                Console.WriteLine("   No se pudo crear la copia de seguridad del archivo original.");
        }

        return citas;
    }

    // ==================== MENÚ ====================

    public static int SeleccionarOpcion()
    {
        Console.WriteLine("\n===== MESA DE PARTES - HOSPITAL =====");
        Console.WriteLine("  1. Registrar una cita");
        Console.WriteLine("  2. Consultar por código o DNI");
        Console.WriteLine("  3. Listar citas");
        Console.WriteLine("  4. Borrar citas del archivo");
        Console.WriteLine("  5. Salir");
        return Entero("Seleccione una opción (1-5): ", 1, 5);
    }

    // ==================== REGISTRAR ====================

    public static void RegistrarCitas()
    {
        do
        {
            RegistrarUna();
        }
        while (SiNo("\n¿Desea registrar otra cita? (si/no): "));
    }

    private static void RegistrarUna()
    {
        Console.WriteLine("\n===== REGISTRO DE CITA =====");

        string nombre = Nombre("Nombres: ");
        string apellido = Nombre("Apellidos: ");
        string dni = Dni("DNI (8 dígitos): ");
        string telefono = Telefono("Número de teléfono (9 dígitos): ");

        int posSeguro = Opcion("¿Tiene seguro de salud?", Seguros);
        string seguro = Seguros[posSeguro];

        Medico medico = ElegirMedico();

        // La fecha y hora son las del momento del registro (sin segundos)
        DateTime ahora = DateTime.Now;
        ahora = new DateTime(ahora.Year, ahora.Month, ahora.Day, ahora.Hour, ahora.Minute, 0);

        // Evita registrar a la misma persona dos veces en la misma área el mismo día
        if (Citas.Any(c => c.Paciente.Dni == dni && c.Medico.Codigo == medico.Codigo && c.FechaHora.Date == ahora.Date))
        {
            Error($"El DNI {dni} ya tiene una cita de hoy en {medico.Especialidad}.");
            return;
        }

        // Código: último número usado + 1 (nunca se repite)
        int numero = Citas.Count == 0 ? 1 : Citas.Max(c => c.Numero) + 1;

        // Turno: siguiente número de atención con ese médico durante el día
        int turno = Citas
            .Where(c => c.Medico.Codigo == medico.Codigo && c.FechaHora.Date == ahora.Date)
            .Select(c => c.Turno)
            .DefaultIfEmpty(0)
            .Max() + 1;

        var paciente = new Paciente(nombre, apellido, dni, telefono, seguro);
        var cita = new Cita(numero, paciente, medico, ahora, turno);

        Console.WriteLine("\n----- Verifique los datos -----");
        Console.WriteLine(cita.Resumen());

        if (!SiNo("\n¿Los datos son correctos? (si/no): "))
        {
            Console.WriteLine("Registro cancelado. No se guardó nada.");
            return;
        }

        Citas.Add(cita);

        if (Guardar())
            Console.WriteLine($"\nCita registrada y guardada. Su código es {cita.Codigo}, turno {turno} en {medico.Especialidad}.");
        else
            Console.WriteLine($"\nCita registrada ({cita.Codigo}), pero NO se pudo guardar en el archivo.");
    }

    private static Medico ElegirMedico()
    {
        var areas = Medicos.Select(m => $"{m.Especialidad} (Dr(a). {m.Nombre})").ToList();
        int pos = Opcion("¿A qué área del hospital se dirige?", areas);
        return Medicos[pos];
    }

    // ==================== CONSULTAR ====================

    public static void BuscarPorCodigo()
    {
        Console.WriteLine("\n===== CONSULTA DE CITA =====");
        string texto = Texto("Ingrese el código (ejemplo 1 o CIT-0001) o el DNI: ");
        texto = texto.Replace("CIT-", "", StringComparison.OrdinalIgnoreCase).Trim();

        if (texto.Length == 0 || !texto.All(c => c is >= '0' and <= '9'))
        {
            Error("Solo se permiten números (código de cita o DNI de 8 dígitos).");
            return;
        }

        List<Cita> resultado;
        if (texto.Length == 8)
        {
            resultado = Citas.Where(c => c.Paciente.Dni == texto).ToList();
        }
        else if (texto.Length <= 7)
        {
            int numero = int.Parse(texto, CultureInfo.InvariantCulture);
            resultado = Citas.Where(c => c.Numero == numero).ToList();
        }
        else
        {
            Error("Dato no válido: el DNI tiene 8 números y el código máximo 7.");
            return;
        }

        if (resultado.Count == 0)
        {
            Error("No se encontró ninguna cita con ese dato.");
            return;
        }

        Console.WriteLine($"\n===== CITAS ENCONTRADAS: {resultado.Count} =====");
        foreach (var c in resultado)
        {
            Console.WriteLine(c.Resumen());
            Console.WriteLine("  ---------------------------------");
        }
    }

    // ==================== LISTAR ====================

    public static void ListarCitas()
    {
        if (Citas.Count == 0)
        {
            Console.WriteLine("\nNo hay citas registradas todavía.");
            return;
        }

        int opcion = Opcion("===== LISTAR CITAS =====", new[]
        {
            "Por orden de registro",
            "Alfabético (por apellido)",
            "Solo las de hoy"
        });

        switch (opcion)
        {
            case 0:
                ImprimirTabla(Citas, "CITAS POR ORDEN DE REGISTRO");
                break;
            case 1:
                var comparador = StringComparer.Create(CultureInfo.InvariantCulture, ignoreCase: true);
                var ordenadas = Citas.OrderBy(c => c.Paciente.Apellido, comparador)
                                     .ThenBy(c => c.Paciente.Nombre, comparador)
                                     .ThenBy(c => c.Numero);
                ImprimirTabla(ordenadas, "CITAS EN ORDEN ALFABÉTICO (A-Z)");
                break;
            default:
                var hoy = Citas.Where(c => c.FechaHora.Date == DateTime.Today).ToList();
                if (hoy.Count == 0)
                    Console.WriteLine("\nHoy todavía no hay citas registradas.");
                else
                    ImprimirTabla(hoy, "CITAS DE HOY");
                break;
        }
    }

    private static string Cortar(string texto, int max)
    {
        return texto.Length <= max ? texto : texto.Substring(0, max);
    }

    private static void ImprimirTabla(IEnumerable<Cita> lista, string titulo)
    {
        var filas = lista.ToList();

        Console.WriteLine($"\n===== {titulo} =====");
        Console.WriteLine($"{"Código",-10}{"Fecha y hora",-18}{"Turno",-7}{"Paciente",-28}{"DNI",-10}{"Teléfono",-11}{"Seguro",-11}{"Área",-17}");
        Console.WriteLine(new string('-', 112));

        foreach (var c in filas)
        {
            string fecha = c.FechaHora.ToString(Cita.FormatoFechaHora, CultureInfo.InvariantCulture);
            string paciente = Cortar(c.Paciente.NombreCompleto, 27);
            string seguro = Cortar(c.Paciente.Seguro, 10);
            string area = Cortar(c.Area, 16);

            Console.WriteLine($"{c.Codigo,-10}{fecha,-18}{c.Turno,-7}{paciente,-28}{c.Paciente.Dni,-10}{c.Paciente.Telefono,-11}{seguro,-11}{area,-17}");
        }

        Console.WriteLine(new string('-', 112));
        Console.WriteLine($"Total: {filas.Count} cita(s).");
    }

    // ==================== BORRAR ====================

    public static void BorrarCitas()
    {
        if (Citas.Count == 0)
        {
            Console.WriteLine("\nNo hay citas para borrar.");
            return;
        }

        int opcion = Opcion("===== BORRAR CITAS =====", new[]
        {
            "Borrar solo las citas de días anteriores",
            "Borrar TODAS las citas (empezar de cero)",
            "Cancelar"
        });

        if (opcion == 2)
        {
            Console.WriteLine("No se borró nada.");
            return;
        }

        int aBorrar = opcion == 0
            ? Citas.Count(c => c.FechaHora.Date < DateTime.Today)
            : Citas.Count;

        if (aBorrar == 0)
        {
            Console.WriteLine("No hay citas de días anteriores para borrar.");
            return;
        }

        if (!SiNo($"Se borrarán {aBorrar} cita(s). ¿Está seguro? (si/no): "))
        {
            Console.WriteLine("No se borró nada.");
            return;
        }

        // Copia de seguridad antes de borrar
        if (Respaldar())
            Console.WriteLine($"Copia de seguridad guardada en: {Ruta}.bak");
        else
            Console.WriteLine("Aviso: no se pudo crear la copia de seguridad.");

        if (opcion == 0)
            Citas.RemoveAll(c => c.FechaHora.Date < DateTime.Today);
        else
            Citas.Clear();

        if (Guardar())
            Console.WriteLine($"Listo: se borraron {aBorrar} cita(s) y el archivo quedó actualizado.");
        else
            Console.WriteLine("Se borraron en memoria, pero NO se pudo actualizar el archivo.");
    }

    // ==================== LECTURA DEL TECLADO ====================
    // Todas repiten la pregunta hasta que el dato sea válido

    public static void Error(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"    ERROR: {mensaje}");
        Console.ResetColor();
    }

    private static string Texto(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string? linea = Console.ReadLine();
            if (linea is null)
                throw new EndOfStreamException("Se cerró la entrada de datos del teclado.");

            string texto = linea.Trim();
            if (texto.Length > 0) return texto;
            Error("El campo no puede estar vacío.");
        }
    }

    private static int Entero(string mensaje, int min, int max)
    {
        while (true)
        {
            string texto = Texto(mensaje);
            if (!int.TryParse(texto, NumberStyles.Integer, CultureInfo.InvariantCulture, out int valor))
                Error("Debe escribir un número entero (sin letras ni símbolos).");
            else if (valor < min || valor > max)
                Error($"El número debe estar entre {min} y {max}.");
            else
                return valor;
        }
    }

    // Muestra una lista numerada y devuelve la posición elegida (empieza en 0)
    private static int Opcion(string titulo, IReadOnlyList<string> opciones)
    {
        Console.WriteLine($"\n{titulo}");
        for (int i = 0; i < opciones.Count; i++)
            Console.WriteLine($"  {i + 1}. {opciones[i]}");

        return Entero($"Seleccione una opción (1-{opciones.Count}): ", 1, opciones.Count) - 1;
    }

    private static string Nombre(string mensaje)
    {
        while (true)
        {
            string texto = Paciente.NormalizarNombre(Texto(mensaje));
            if (Paciente.NombreValido(texto)) return texto;
            Error("Solo letras y espacios (entre 2 y 30 caracteres).");
        }
    }

    private static string Dni(string mensaje)
    {
        while (true)
        {
            string texto = Texto(mensaje).Replace(" ", "");
            if (Paciente.DniValido(texto)) return texto;
            Error("El DNI debe tener exactamente 8 números.");
        }
    }

    private static string Telefono(string mensaje)
    {
        while (true)
        {
            string texto = Texto(mensaje).Replace(" ", "");
            if (Paciente.TelefonoValido(texto)) return texto;
            Error("El teléfono debe tener 9 números y empezar con 9.");
        }
    }

    private static bool SiNo(string mensaje)
    {
        while (true)
        {
            string texto = Texto(mensaje).ToLowerInvariant();
            if (texto is "si" or "sí" or "s") return true;
            if (texto is "no" or "n") return false;
            Error("Responda únicamente 'si' o 'no'.");
        }
    }
}