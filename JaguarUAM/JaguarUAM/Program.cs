// ============================================================================
//  SERGIO CABRERA - MODULO DATOS Y PERSONAJE
//  Contiene structs, arreglos globales, inicializacion, magia, experiencia y niveles.
// ============================================================================
struct Jaguar
{
    public string nombre;
    public int hp;
    public int hpMax;
    public int ataque;
    public int defensa;
    public int nivel;
    public int exp;
    public int magia;
    public string[] habilidades; // string[4]
}

struct Enemigo
{
    public string nombre;
    public int hp;
    public int hpMax;
    public int ataque;
    public int defensa;
    public int expReward;
    public string descripcion;
    public bool vivo;
}

struct Zona
{
    public string nombre;
    public string descripcion;
    public string historia;
    public bool visitada;
    public int[] indiceEnemigos; // int[2]
    public int cantidadEnemigos;
}

class Program
{
    // --------------------------------------------------------------------
    //  ARREGLOS GLOBALES Y ESTADO COMPARTIDO
    // --------------------------------------------------------------------
    static Enemigo[] enemigos = new Enemigo[7];
    static Zona[] zonas = new Zona[5];
    static Jaguar jaguar;
    static int zonaActual = 0;
    static Random rng = new Random();

    // NUEVO: inventario con arreglos paralelos (sin List<T>)
    static string[] itemNombres = { "Refresco UAM", "Cafe Cargado", "Empanada Magica" };
    static int[] itemCantidad = new int[3]; // se llena en InicializarJaguar

    // NUEVO: costos de maná de cada habilidad (indices alineados con habilidades[])
    static int[] costoHabilidad = { 10, 12, 16, 25 };

    // ====================================================================
    //  MODULO DATOS: Inicializacion
    // ====================================================================
    static void InicializarEnemigos()
    {
        enemigos[0].nombre = "Vigilante Corrupto";
        enemigos[0].hp = 45; enemigos[0].hpMax = 45;
        enemigos[0].ataque = 13; enemigos[0].defensa = 2;
        enemigos[0].expReward = 20; enemigos[0].vivo = true;
        enemigos[0].descripcion = "Cobra mordida en cada porton. Te niega el paso si no le pagas.";

        enemigos[1].nombre = "Profe de Calculo";
        enemigos[1].hp = 60; enemigos[1].hpMax = 60;
        enemigos[1].ataque = 15; enemigos[1].defensa = 4;
        enemigos[1].expReward = 30; enemigos[1].vivo = true;
        enemigos[1].descripcion = "Lanza limites infinitos. Puede SUBIR LA DIFICULTAD y volverse mas fuerte.";

        enemigos[2].nombre = "Examen Sorpresa";
        enemigos[2].hp = 55; enemigos[2].hpMax = 55;
        enemigos[2].ataque = 17; enemigos[2].defensa = 3;
        enemigos[2].expReward = 30; enemigos[2].vivo = true;
        enemigos[2].descripcion = "Aparece sin avisar. Puede CONFUNDIRTE y arruinar tu siguiente zarpazo.";

        enemigos[3].nombre = "Cocinera Enojada";
        enemigos[3].hp = 75; enemigos[3].hpMax = 75;
        enemigos[3].ataque = 19; enemigos[3].defensa = 5;
        enemigos[3].expReward = 45; enemigos[3].vivo = true;
        enemigos[3].descripcion = "Defiende la cafeteria con cucharon de hierro. A veces COME y se cura.";

        enemigos[4].nombre = "Decano Estricto";
        enemigos[4].hp = 90; enemigos[4].hpMax = 90;
        enemigos[4].ataque = 23; enemigos[4].defensa = 8;
        enemigos[4].expReward = 70; enemigos[4].vivo = true;
        enemigos[4].descripcion = "Revisa tu carnet y tu asistencia. Golpea duro cuando se enoja.";

        enemigos[5].nombre = "Guardia de Biblioteca";
        enemigos[5].hp = 100; enemigos[5].hpMax = 100;
        enemigos[5].ataque = 27; enemigos[5].defensa = 10;
        enemigos[5].expReward = 90; enemigos[5].vivo = true;
        enemigos[5].descripcion = "Exige silencio absoluto. Castiga cada susurro con furia.";

        enemigos[6].nombre = "El Director en la Sombra";
        enemigos[6].hp = 140; enemigos[6].hpMax = 140;
        enemigos[6].ataque = 24; enemigos[6].defensa = 9;
        enemigos[6].expReward = 150; enemigos[6].vivo = true;
        enemigos[6].descripcion = "El jefe final. Canaliza energia oscura y desata el DECRETO FINAL. No puedes huir de el.";
    }

    static void InicializarZonas()
    {
        zonas[0].nombre = "Entrada Principal";
        zonas[0].descripcion = "El porton de acceso a la UAM.";
        zonas[0].historia = "Llegas a la entrada de la UAM. El campus esta tomado por fuerzas " +
                            "oscuras. Solo tu, el Jaguar, puedes liberarlo zona por zona.";
        zonas[0].visitada = false;
        zonas[0].indiceEnemigos = new int[2] { 0, -1 };
        zonas[0].cantidadEnemigos = 1;

        zonas[1].nombre = "Pasillos Academicos";
        zonas[1].descripcion = "Largos pasillos llenos de aulas.";
        zonas[1].historia = "Avanzas por los pasillos. El eco de tizas y examenes retumba. " +
                            "Aqui te esperan los guardianes del conocimiento.";
        zonas[1].visitada = false;
        zonas[1].indiceEnemigos = new int[2] { 1, 2 };
        zonas[1].cantidadEnemigos = 2;

        zonas[2].nombre = "Cafeteria UAM";
        zonas[2].descripcion = "El corazon social del campus.";
        zonas[2].historia = "El olor a comida te recibe, pero la Cocinera no esta de humor. " +
                            "Nadie pasa sin probar su furia.";
        zonas[2].visitada = false;
        zonas[2].indiceEnemigos = new int[2] { 3, -1 };
        zonas[2].cantidadEnemigos = 1;

        zonas[3].nombre = "Auditorio";
        zonas[3].descripcion = "El gran salon de actos.";
        zonas[3].historia = "Las luces del auditorio se apagan. Las autoridades del campus " +
                            "se interponen entre tu y la verdad.";
        zonas[3].visitada = false;
        zonas[3].indiceEnemigos = new int[2] { 4, 5 };
        zonas[3].cantidadEnemigos = 2;

        zonas[4].nombre = "Biblioteca - Oficina del Director";
        zonas[4].descripcion = "El ultimo bastion de la sombra.";
        zonas[4].historia = "En lo mas profundo de la biblioteca, entre estanterias infinitas, " +
                            "aguarda El Director en la Sombra. El destino de la UAM se decide aqui.";
        zonas[4].visitada = false;
        zonas[4].indiceEnemigos = new int[2] { 6, -1 };
        zonas[4].cantidadEnemigos = 1;
    }

    static void InicializarJaguar()
    {
        jaguar.nombre = "Jaguarcito";
        jaguar.hp = 100; jaguar.hpMax = 100;
        jaguar.ataque = 20; jaguar.defensa = 5;
        jaguar.nivel = 1; jaguar.exp = 0; jaguar.magia = 30;
        jaguar.habilidades = new string[4];
        jaguar.habilidades[0] = "Zarpazo Feroz";
        jaguar.habilidades[1] = "";
        jaguar.habilidades[2] = "";
        jaguar.habilidades[3] = "";

        // NUEVO: inventario inicial
        itemCantidad[0] = 1; // Refresco UAM
        itemCantidad[1] = 1; // Cafe Cargado
        itemCantidad[2] = 0; // Empanada Magica
    }

    // NUEVO: tope de maná = progresion natural (30 + 10 por nivel)
    static int MagiaMax(Jaguar j)
    {
        return 30 + (j.nivel - 1) * 10;
    }

    // NUEVO: sumar maná sin pasar del tope
    static void RecuperarMagia(ref Jaguar j, int cantidad)
    {
        j.magia += cantidad;
        int tope = MagiaMax(j);
        if (j.magia > tope) j.magia = tope;
    }

    // ====================================================================
    //  MODULO PERSONAJE: Experiencia, niveles
    // ====================================================================
    static void GanarExp(ref Jaguar j, int expGanada)
    {
        j.exp += expGanada;
        Escribir("  + " + expGanada + " EXP", ConsoleColor.Cyan);
        while (j.exp >= j.nivel * 50)
        {
            j.exp -= (j.nivel * 50);
            SubirNivel(ref j);
        }
    }

    static void SubirNivel(ref Jaguar j)
    {
        j.nivel += 1;
        j.hpMax += 15;
        j.hp = j.hpMax;          // cura completo al subir
        j.ataque += 5;
        j.defensa += 2;
        j.magia = MagiaMax(j);   // rellena el mana al nuevo tope
        Escribir("  ¡NIVEL " + j.nivel + "!  Stats mejorados.", ConsoleColor.Yellow);

        if (j.nivel == 2)
        {
            j.habilidades[1] = "Rugido Intimidante";
            Escribir("  Nueva habilidad: " + j.habilidades[1] + " (baja el ataque enemigo)", ConsoleColor.Magenta);
        }
        if (j.nivel == 3)
        {
            j.habilidades[2] = "Garra del Jaguar";
            Escribir("  Nueva habilidad: " + j.habilidades[2] + " (alto critico)", ConsoleColor.Magenta);
        }
        if (j.nivel == 4)
        {
            j.habilidades[3] = "Furia UAM";
            Escribir("  Nueva habilidad: " + j.habilidades[3] + " (golpe devastador)", ConsoleColor.Magenta);
        }
    }

