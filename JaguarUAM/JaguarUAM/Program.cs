// ============================================================================
//  JONAS HODGSON - MODULO UI, ARCHIVOS, UTILIDADES Y MAIN
//  Contiene guardado/cargado, interfaz, menus, validaciones y punto de entrada.
// ============================================================================

//  MODULO PERSISTENCIA: System.IO
// ====================================================================
static void GuardarProgreso(Jaguar j, int zona, Enemigo[] enems)
{
    string[] lineas = new string[19]; // 16 originales + 3 de objetos
    lineas[0] = j.nombre;
    lineas[1] = j.hp.ToString();
    lineas[2] = j.hpMax.ToString();
    lineas[3] = j.ataque.ToString();
    lineas[4] = j.defensa.ToString();
    lineas[5] = j.nivel.ToString();
    lineas[6] = j.exp.ToString();
    lineas[7] = j.magia.ToString();
    lineas[8] = zona.ToString();
    for (int i = 0; i <= 6; i++) lineas[9 + i] = enems[i].vivo.ToString();
    lineas[16] = itemCantidad[0].ToString(); 
    lineas[17] = itemCantidad[1].ToString();
    lineas[18] = itemCantidad[2].ToString();

    try
    {
        File.WriteAllLines("progreso.txt", lineas);
        Escribir("  Progreso guardado correctamente.", ConsoleColor.Green);
    }
    catch (Exception e) { Escribir("  Error al guardar: " + e.Message, ConsoleColor.Red); }
}

static void CargarProgreso(ref Jaguar j, ref int zona, ref Enemigo[] enems)
{
    if (File.Exists("progreso.txt") == false)
    {
        Escribir("  Sin partida guardada. Continuas la partida actual.", ConsoleColor.DarkYellow);
        return;
    }
    try
    {
        string[] lineas = File.ReadAllLines("progreso.txt");
        j.nombre = lineas[0];
        j.hp = int.Parse(lineas[1]); j.hpMax = int.Parse(lineas[2]);
        j.ataque = int.Parse(lineas[3]); j.defensa = int.Parse(lineas[4]);
        j.nivel = int.Parse(lineas[5]); j.exp = int.Parse(lineas[6]);
        j.magia = int.Parse(lineas[7]);
        zona = int.Parse(lineas[8]);
        if (zona < 0) zona = 0;
        if (zona > 4) zona = 4;
        for (int i = 0; i <= 6; i++)
        {
            enems[i].vivo = bool.Parse(lineas[9 + i]);
            if (!enems[i].vivo) enems[i].hp = 0;
        }
        if (lineas.Length >= 19) // compatibilidad con guardados viejos
        {
            itemCantidad[0] = int.Parse(lineas[16]);
            itemCantidad[1] = int.Parse(lineas[17]);
            itemCantidad[2] = int.Parse(lineas[18]);
        }
        Escribir("  Partida cargada correctamente.", ConsoleColor.Green);
    }
    catch (Exception e) { Escribir("  Error al cargar: " + e.Message, ConsoleColor.Red); }
}

// ====================================================================
//  MODULO UI
// ====================================================================
static void Escribir(string texto, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.WriteLine(texto);
    Console.ResetColor();
}

static void EscribirLento(string texto)
{
    foreach (char c in texto) { Console.Write(c); Thread.Sleep(12); }
    Console.WriteLine();
}

static void MostrarBarraHP(int hp, int hpMax)
{
    if (hp < 0) hp = 0;
    int llenas = (int)((double)hp / hpMax * 20);
    if (llenas > 20) llenas = 20;
    int vacias = 20 - llenas;
    StringBuilder barra = new StringBuilder();
    barra.Append("[");
    for (int i = 0; i < llenas; i++) barra.Append("\u2588");
    for (int i = 0; i < vacias; i++) barra.Append("\u2591");
    barra.Append("]");
    ConsoleColor color;
    if (hp > hpMax * 0.5) color = ConsoleColor.Green;
    else if (hp > hpMax * 0.2) color = ConsoleColor.Yellow;
    else color = ConsoleColor.Red;
    Escribir("HP " + barra.ToString() + " " + hp + "/" + hpMax, color);
}

static void MostrarMenuPrincipal()
{
    Console.Clear();
    Escribir("######################################", ConsoleColor.DarkCyan);
    Escribir("#       JAGUARCITO UAM - RPG          #", ConsoleColor.DarkCyan);
    Escribir("######################################", ConsoleColor.DarkCyan);
    Console.WriteLine("  Zona actual: " + zonas[zonaActual].nombre);
    Console.Write("  ");
    MostrarBarraHP(jaguar.hp, jaguar.hpMax);
    Console.WriteLine("  Magia: " + jaguar.magia + "/" + MagiaMax(jaguar));
    Console.WriteLine();
    Console.WriteLine("  [1] Explorar zona actual");
    Console.WriteLine("  [2] Ver stats del Jaguar");
    Console.WriteLine("  [3] Ver mapa del campus UAM");
    Console.WriteLine("  [4] Volver a zona anterior");
    Console.WriteLine("  [5] Guardar progreso");
    Console.WriteLine("  [6] Cargar progreso guardado");
    Console.WriteLine("  [7] Salir del juego");
    Console.WriteLine();
    Console.Write("  Elige una opcion: ");
}

static void MostrarPanelCombate(Jaguar j, Enemigo e, bool bossCargando)
{
    Console.WriteLine();
    Escribir("---------- COMBATE ----------", ConsoleColor.DarkRed);
    Console.Write("  " + e.nombre + "  ");
    MostrarBarraHP(e.hp, e.hpMax);
    if (bossCargando) Escribir("  ¡CUIDADO! El jefe esta por desatar un golpe enorme.", ConsoleColor.Yellow);
    Console.Write("  " + j.nombre + "  ");
    MostrarBarraHP(j.hp, j.hpMax);
    Console.WriteLine("  Magia: " + j.magia + "/" + MagiaMax(j) +
                      "   Items: R" + itemCantidad[0] + " C" + itemCantidad[1] + " E" + itemCantidad[2]);
    Console.WriteLine();
    Console.WriteLine("  [1] Zarpazo (basico, +6 magia)");
    Console.WriteLine("  [2] Habilidad especial");
    Console.WriteLine("  [3] Curar (-12 magia)");
    Console.WriteLine("  [4] Usar objeto");
    Console.WriteLine("  [5] Huir");
    Console.Write("  Accion: ");
}

static void MostrarArteEnemigo(bool esJefe)
{
    if (esJefe)
    {
        Escribir(@"      .-=========-.", ConsoleColor.DarkMagenta);
        Escribir(@"      \'-=======-'/", ConsoleColor.DarkMagenta);
        Escribir(@"      _|   .=.   |_", ConsoleColor.DarkMagenta);
        Escribir(@"     ((|  {{1}}  |))", ConsoleColor.DarkMagenta);
        Escribir(@"      \|   /|\   |/", ConsoleColor.DarkMagenta);
        Escribir(@"       \__ '`' __/", ConsoleColor.DarkMagenta);
    }
    else
    {
        Escribir(@"        .-.   .-.", ConsoleColor.DarkRed);
        Escribir(@"       (   `-'   )", ConsoleColor.DarkRed);
        Escribir(@"        `-.   .-'", ConsoleColor.DarkRed);
        Escribir(@"          /   \", ConsoleColor.DarkRed);
        Escribir(@"         '     '", ConsoleColor.DarkRed);
    }
}

static void MostrarVictoria()
{
    Console.Clear();
    Escribir(@"  __     ___ ____ _____ ___  ____  ___    _    ", ConsoleColor.Yellow);
    Escribir(@"  \ \   / |_ |/ ___|_   _/ _ \|  _ \|_ _|  / \   ", ConsoleColor.Yellow);
    Escribir(@"   \ \ / / | | |     | || | | | |_) || |  / _ \  ", ConsoleColor.Yellow);
    Escribir(@"    \ V /  | | |___  | || |_| |  _ < | | / ___ \ ", ConsoleColor.Yellow);
    Escribir(@"     \_/  |___\____| |_| \___/|_| \_|___/_/   \_\", ConsoleColor.Yellow);
    Console.WriteLine();
    Escribir("  ¡El Director en la Sombra ha caido!", ConsoleColor.Green);
    Escribir("  El Jaguar ha liberado el campus de la UAM.", ConsoleColor.Green);
    Console.WriteLine();
    MostrarStats(jaguar);
    GuardarRanking(jaguar);
    Console.WriteLine();
    Pausa();
}

static void MostrarGameOver()
{
    Console.Clear();
    Escribir(@"   ____    _    __  __ _____ ", ConsoleColor.Red);
    Escribir(@"  / ___|  / \  |  \/  | ____|", ConsoleColor.Red);
    Escribir(@" | |  _  / _ \ | |\/| |  _|  ", ConsoleColor.Red);
    Escribir(@" | |_| |/ ___ \| |  | | |___ ", ConsoleColor.Red);
    Escribir(@"  \____/_/   \_|_|  |_|_____| OVER", ConsoleColor.Red);
    Console.WriteLine();
    Escribir("  El Jaguar ha caido en " + zonas[zonaActual].nombre + ".", ConsoleColor.DarkRed);
    Escribir("  Tu progreso guardado sigue disponible para reintentar.", ConsoleColor.DarkYellow);
    Console.WriteLine();
    Pausa();
}

static void GuardarRanking(Jaguar j)
{
    try
    {
        int puntaje = j.nivel * 1000 + j.exp + j.hp;
        string linea = j.nombre + " - Nivel " + j.nivel + " - Puntaje " + puntaje;
        File.AppendAllLines("ranking.txt", new string[] { linea });
        Escribir("  Puntaje registrado en ranking.txt", ConsoleColor.Cyan);
    }
    catch { }
}

// ====================================================================
//  UTILIDADES
// ====================================================================
static int LeerEntero(int min, int max)
{
    int valor;
    string entrada = Console.ReadLine();
    while (!int.TryParse(entrada, out valor) || valor < min || valor > max)
    {
        Escribir("  Entrada invalida. Ingresa un numero entre " + min + " y " + max + ":", ConsoleColor.DarkYellow);
        entrada = Console.ReadLine();
    }
    return valor;
}

static int Maximo(int a, int b) { return (a > b) ? a : b; }
static int Minimo(int a, int b) { return (a < b) ? a : b; }

static void Pausa()
{
    Console.WriteLine();
    Console.Write("  Presiona ENTER para continuar...");
    Console.ReadLine();
}

// ====================================================================
//  PUNTO DE ENTRADA
// ====================================================================
static void Main(string[] args)
{
    Console.OutputEncoding = Encoding.UTF8;
    Console.Title = "Jaguarcito UAM - RPG del Campus";

    InicializarEnemigos();
    InicializarZonas();
    InicializarJaguar();

    Console.Clear();
    Escribir("######################################", ConsoleColor.DarkCyan);
    Escribir("#   JAGUARCITO UAM - RPG DEL CAMPUS   #", ConsoleColor.DarkCyan);
    Escribir("######################################", ConsoleColor.DarkCyan);
    Console.WriteLine();
    Console.Write("  Ingresa el nombre de tu Jaguar (ENTER = Jaguarcito): ");
    string nombre = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(nombre)) jaguar.nombre = nombre.Trim();

    if (File.Exists("progreso.txt"))
    {
        Console.Write("  Hay una partida guardada. ¿Cargar? [1] Si  [2] No: ");
        int op = LeerEntero(1, 2);
        if (op == 1) CargarProgreso(ref jaguar, ref zonaActual, ref enemigos);
        Pausa();
    }

    BucleJuego();

    Console.WriteLine();
    Escribir("  Gracias por jugar Jaguarcito UAM.", ConsoleColor.Cyan);
}
    }
}
