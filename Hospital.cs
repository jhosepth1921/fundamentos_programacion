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
        new Medico("MED-005", "Pedro Sánchez", "Pediatría")
    };

    public static readonly List<Paciente> Pacientes = new()
    {
        new Paciente("123", "Alessandro Anthony", 19),
        new Paciente("098", "Diego Daniel", 18),
        new Paciente("836", "Jorge Joaquin", 17),
        new Paciente("086", "Rootney Alessandro", 17),
        new Paciente("777", "Junior Jerao", 17)
    };

    public static readonly List<Cita> Citas = new()
    {
        new Cita("001", Pacientes[0], Medicos[0], "30/09/2026"),
        new Cita("002", Pacientes[1], Medicos[1], "01/10/2026"),
        new Cita("003", Pacientes[2], Medicos[2], "02/10/2026"),
        new Cita("004", Pacientes[3], Medicos[3], "03/10/2026"),
        new Cita("005", Pacientes[4], Medicos[4], "04/10/2026")
    };

    // Lee un entero; si el usuario escribe algo no numérico devuelve -1 (en Python daría error)
    private static int LeerEntero(string mensaje)
    {
        Console.Write(mensaje);
        return int.TryParse(Console.ReadLine(), out int valor) ? valor : -1;
    }

    private static string LeerTexto(string mensaje)
    {
        Console.Write(mensaje);
        return Console.ReadLine() ?? "";
    }

    public static int SeleccionarOpcion()
    {
        Console.WriteLine(@"
╔══════════════════  BIENVENIDO AL CENTRO DEL CONSULTORIO DEL HOSPITAL  ══════════════════╗
║                                                                                         ║
║   ╔─────────────────────────────────────────────────────────────────────────────────╗   ║
║   │  1. Consultar datos de una cita ........................ seleccione (N. 1)      │   ║
║   │  2. Registrar una cita ................................. seleccione (N. 2)      │   ║
║   ╚─────────────────────────────────────────────────────────────────────────────────╝   ║
║                                                                                         ║
╚═════════════════════════════════════════════════════════════════════════════════════════╝
");

        int alternativa = LeerEntero("Seleccione una opción (con un número): ");

        switch (alternativa)
        {
            case 1:
                return 1;
            case 2:
                return 2;
            default:
                Console.WriteLine("La opción seleccionada no es válida");
                return 0;
        }
    }

    public static Medico ElegirMedico()
    {
        Console.WriteLine("\n====== ÁREAS DEL HOSPITAL ======");
        Console.WriteLine("1. Nutrición");
        Console.WriteLine("2. Psicología");
        Console.WriteLine("3. Odontología");
        Console.WriteLine("4. Cardiología");
        Console.WriteLine("5. Pediatría");

        int opcion = LeerEntero("Seleccione un área del hospital (con un número): ");

        switch (opcion)
        {
            case 1: return Medicos[0];
            case 2: return Medicos[1];
            case 3: return Medicos[2];
            case 4: return Medicos[3];
            case 5: return Medicos[4];
            default:
                Console.WriteLine("Opción inválida");
                Environment.Exit(0);
                return Medicos[0]; // nunca se ejecuta, pero C# exige un return
        }
    }

    public static List<Cita> GenerarCita()
    {
        var nuevasCitas = new List<Cita>();
        bool continuar = true;

        while (continuar)
        {
            Console.WriteLine("\n===== REGISTRO DEL PACIENTE =====");
            string codigoCita = LeerTexto("Ingrese código de la cita CIT-(MAX. 3 dígitos): ");
            string codigo = LeerTexto("Ingrese código del paciente PAC-(MAX. 3 dígitos): ");
            string nombre = LeerTexto("Ingrese su nombre: ");
            int edad = LeerEntero("Ingresar edad: ");

            var paciente = new Paciente(codigo, nombre, edad);
            Medico medico = ElegirMedico();
            string fecha = LeerTexto("Ingresar fecha en la que desea reservar una cita (dd/mm/aaaa): ");

            var cita = new Cita(codigoCita, paciente, medico, fecha);
            nuevasCitas.Add(cita);

            continuar = LeerTexto("\n¿Desea registrar otra cita? (si/no): ").ToLower() == "si";
        }

        Console.WriteLine("\n===== RESUMEN DE CITAS REGISTRADAS =====");
        foreach (var cita in nuevasCitas)
        {
            Console.WriteLine(cita.Resumen());
            Console.WriteLine("\n=================================");
        }
        return nuevasCitas;
    }

    public static void BuscarPorCodigo(List<Cita> citas)
    {
        Console.WriteLine("\n========== BÚSQUEDA DE CITAS REGISTRADAS ==========");
        string codigo = LeerTexto("\nIngrese el código de la cita CIT-: ");

        foreach (var cita in citas)
        {
            if (cita.Codigo == codigo)
            {
                Console.WriteLine("\n===== CITA ENCONTRADA =====");
                Console.WriteLine(cita.Resumen());
                return;
            }
        }

        Console.WriteLine("\nNo se encontró ninguna cita con ese código.");
    }
}
