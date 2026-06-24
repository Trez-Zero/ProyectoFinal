// ============================================================================
//  JAGUARCITO UAM - RPG DEL CAMPUS  (version 2 - jugabilidad mejorada)
//  Introduccion a la Programacion (C#) - Universidad Americana
//  Integrantes: Sergio Cabrera - Jonas Hodgson - Ricardo Hamad - Ervin Rocha
//  Docente: Jose Duran
// ============================================================================

using System;
using System.IO;
using System.Text;
using System.Threading;

namespace JaguarcitoUAM
{
    // ------------------------------------------------------------------------
    //  STRUCTS (Tipos de datos)  -- sin cambios respecto a la Actividad 2
    // ------------------------------------------------------------------------
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
            j.magia = MagiaMax(j);   // NUEVO: rellena el mana al nuevo tope
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

        // ====================================================================
        //  MODULO COMBATE  (rediseñado)
        // ====================================================================
        static bool IniciarCombate(ref Jaguar j, ref Enemigo enemigo, bool esJefe)
        {
            // --- Variables de estado del combate (locales, sin tocar los structs) ---
            int debuffAtaqueEnemigo = 0; // NUEVO: reduce el ataque del enemigo (Rugido)
            bool confundido = false;     // NUEVO: tu proximo zarpazo hace mitad de daño
            int bossTurno = 0;           // NUEVO: contador de turnos del jefe
            bool bossCargando = false;   // NUEVO: el jefe prepara su golpe pesado
            bool bossYaCuro = false;     // NUEVO: el jefe solo se cura una vez

            Console.Clear();
            MostrarArteEnemigo(esJefe);
            Escribir("¡Aparece " + enemigo.nombre + "!", ConsoleColor.Red);
            EscribirLento(enemigo.descripcion);
            Pausa();

            while (j.hp > 0 && enemigo.hp > 0)
            {
                // ===================== TURNO DEL JUGADOR =====================
                MostrarPanelCombate(j, enemigo, bossCargando);
                int accion = LeerEntero(1, 5);
                bool turnoConsumido = true; // si la accion fue "real", el enemigo responde

                if (accion == 1) // ---- Zarpazo (basico) ----
                {
                    int danio = Maximo(1, j.ataque - enemigo.defensa);
                    if (rng.Next(0, 100) < 15) { danio *= 2; Escribir("  ¡GOLPE CRITICO!", ConsoleColor.Yellow); }
                    if (confundido)
                    {
                        danio = Maximo(1, danio / 2);
                        Escribir("  Estabas confundido: el zarpazo sale debil.", ConsoleColor.DarkYellow);
                        confundido = false;
                    }
                    enemigo.hp -= danio;
                    RecuperarMagia(ref j, 6); // NUEVO: el basico recupera mana
                    Escribir("  Zarpazo: " + danio + " de dano  (+6 magia)", ConsoleColor.Green);
                }
                else if (accion == 2) // ---- Habilidad especial (submenu) ----
                {
                    turnoConsumido = EjecutarHabilidad(ref j, ref enemigo, ref debuffAtaqueEnemigo);
                }
                else if (accion == 3) // ---- Curar ----
                {
                    if (j.magia >= 12)
                    {
                        int cura = 25 + j.nivel * 5; // NUEVO: cura escala con el nivel
                        j.hp = Minimo(j.hp + cura, j.hpMax);
                        j.magia -= 12;
                        Escribir("  Te curas " + cura + " HP.  (-12 magia)", ConsoleColor.Green);
                    }
                    else
                    {
                        Escribir("  Sin magia suficiente para curar (necesitas 12).", ConsoleColor.DarkYellow);
                        turnoConsumido = false; // no se gasta tu turno por una accion fallida
                    }
                }
                else if (accion == 4) // ---- Usar objeto ----
                {
                    turnoConsumido = UsarObjeto(ref j);
                }
                else if (accion == 5) // ---- Huir ----
                {
                    if (esJefe)
                    {
                        Escribir("  ¡El Director bloquea la salida! No puedes huir.", ConsoleColor.DarkRed);
                        turnoConsumido = false;
                    }
                    else if (rng.Next(0, 100) < 45) // 45% de exito
                    {
                        enemigo.hp = enemigo.hpMax; // NUEVO: el enemigo se recupera (no hay cheese)
                        Escribir("  Te retiras al menu. El enemigo recupera su HP.", ConsoleColor.DarkYellow);
                        Pausa();
                        return false;
                    }
                    else
                    {
                        Escribir("  ¡No pudiste escapar!", ConsoleColor.DarkRed);
                    }
                }

                if (enemigo.hp <= 0) break;
                if (!turnoConsumido) continue; // accion libre: el enemigo no actua

                // ===================== TURNO DEL ENEMIGO =====================
                int atkBase = Maximo(1, enemigo.ataque - debuffAtaqueEnemigo);

                // Enojo: bajo 30% de HP, el enemigo pega mas fuerte
                bool enojado = enemigo.hp <= enemigo.hpMax * 0.30;
                if (enojado) atkBase = (int)(atkBase * 1.25);

                if (esJefe)
                {
                    bossTurno++;

                    // El jefe se cura una vez al bajar del 50%
                    if (!bossYaCuro && enemigo.hp <= enemigo.hpMax * 0.5)
                    {
                        enemigo.hp = Minimo(enemigo.hp + 30, enemigo.hpMax);
                        bossYaCuro = true;
                        Escribir("  El Director invoca expedientes y se cura 30 HP.", ConsoleColor.DarkMagenta);
                    }
                    else if (bossCargando)
                    {
                        // Desata el golpe cargado: muy fuerte (telegrafiado el turno anterior)
                        int golpe = Maximo(1, (int)(atkBase * 1.8) - j.defensa);
                        j.hp -= golpe;
                        bossCargando = false;
                        Escribir("  ¡DECRETO FINAL!  " + golpe + " de dano devastador.", ConsoleColor.Red);
                    }
                    else if (bossTurno % 3 == 0)
                    {
                        // Telegrafia: avisa que cargara el golpe (el jugador debe prepararse)
                        bossCargando = true;
                        Escribir("  El Director canaliza energia oscura... (prepara un golpe enorme)", ConsoleColor.DarkMagenta);
                    }
                    else
                    {
                        int golpe = Maximo(1, rng.Next(atkBase - 3, atkBase + 6) - j.defensa);
                        j.hp -= golpe;
                        Escribir("  " + enemigo.nombre + " ataca: " + golpe + " de dano", ConsoleColor.Red);
                    }
                }
                else
                {
                    // IA con movimiento especial segun el enemigo (~22% de probabilidad)
                    bool usoEspecial = AccionEspecialEnemigo(ref enemigo, ref j, ref confundido);
                    if (!usoEspecial)
                    {
                        int golpe = Maximo(1, rng.Next(atkBase - 3, atkBase + 6) - j.defensa);
                        j.hp -= golpe;
                        string extra = enojado ? "  (¡enojado!)" : "";
                        Escribir("  " + enemigo.nombre + " ataca: " + golpe + " de dano" + extra, ConsoleColor.Red);
                    }
                }
                Pausa();
            }

            // ===================== RESOLUCION =====================
            if (j.hp > 0)
            {
                enemigo.hp = 0;
                enemigo.vivo = false;
                Console.WriteLine();
                Escribir("  ¡Derrotaste a " + enemigo.nombre + "!", ConsoleColor.Green);
                GanarExp(ref j, enemigo.expReward);

                // NUEVO: botin garantizado al ganar (recompensa explorar/pelear)
                int botin = rng.Next(0, 3);
                itemCantidad[botin]++;
                Escribir("  Obtienes 1x " + itemNombres[botin], ConsoleColor.Cyan);

                Pausa();
                return true;
            }
            return false; // derrota
        }

        // NUEVO: submenu de habilidades, cada una con efecto distinto
        static bool EjecutarHabilidad(ref Jaguar j, ref Enemigo e, ref int debuffAtaqueEnemigo)
        {
            // Listar habilidades desbloqueadas
            int[] disponibles = new int[4];
            int cuenta = 0;
            Console.WriteLine();
            Escribir("  -- Habilidades --", ConsoleColor.Magenta);
            for (int i = 0; i < 4; i++)
            {
                if (j.habilidades[i] != null && j.habilidades[i] != "")
                {
                    disponibles[cuenta] = i;
                    cuenta++;
                    Console.WriteLine("   [" + cuenta + "] " + j.habilidades[i] + "  (cuesta " + costoHabilidad[i] + " magia)");
                }
            }
            Console.WriteLine("   [0] Volver");
            Console.Write("  Elige: ");
            int sel = LeerEntero(0, cuenta);
            if (sel == 0) return false; // cancelar: no gasta turno

            int idx = disponibles[sel - 1];
            if (j.magia < costoHabilidad[idx])
            {
                Escribir("  Sin magia suficiente.", ConsoleColor.DarkYellow);
                return false; // no gasta turno
            }
            j.magia -= costoHabilidad[idx];

            int danio;
            if (idx == 0) // Zarpazo Feroz: daño solido, ignora parte de la defensa
            {
                danio = Maximo(1, (j.ataque + 8) - Maximo(0, e.defensa - 3));
                e.hp -= danio;
                Escribir("  Zarpazo Feroz: " + danio + " de dano", ConsoleColor.Magenta);
            }
            else if (idx == 1) // Rugido Intimidante: poco daño + baja el ataque enemigo
            {
                danio = Maximo(1, (j.ataque - 4) - e.defensa);
                e.hp -= danio;
                debuffAtaqueEnemigo += 4;
                Escribir("  Rugido Intimidante: " + danio + " de dano. ¡El enemigo se acobarda! (-4 ataque)", ConsoleColor.Magenta);
            }
            else if (idx == 2) // Garra del Jaguar: alto daño, 40% critico
            {
                danio = Maximo(1, (j.ataque + 16) - e.defensa);
                if (rng.Next(0, 100) < 40) { danio = (int)(danio * 1.6); Escribir("  ¡CRITICO DE GARRA!", ConsoleColor.Yellow); }
                e.hp -= danio;
                Escribir("  Garra del Jaguar: " + danio + " de dano", ConsoleColor.Magenta);
            }
            else // Furia UAM: golpe devastador
            {
                danio = Maximo(1, (j.ataque + 30) - e.defensa);
                e.hp -= danio;
                Escribir("  ¡FURIA UAM!: " + danio + " de dano devastador", ConsoleColor.Magenta);
            }
            return true;
        }

        // NUEVO: movimientos especiales propios de cada enemigo
        static bool AccionEspecialEnemigo(ref Enemigo e, ref Jaguar j, ref bool confundido)
        {
            if (rng.Next(0, 100) >= 22) return false; // 78% ataca normal

            if (e.nombre == "Profe de Calculo")
            {
                e.ataque += 3;
                Escribir("  El Profe SUBE LA DIFICULTAD: su ataque aumenta.", ConsoleColor.DarkRed);
                return true;
            }
            if (e.nombre == "Examen Sorpresa")
            {
                confundido = true;
                Escribir("  ¡Examen Sorpresa te CONFUNDE! Tu proximo zarpazo sera debil.", ConsoleColor.DarkRed);
                return true;
            }
            if (e.nombre == "Cocinera Enojada")
            {
                e.hp = Minimo(e.hp + 12, e.hpMax);
                Escribir("  La Cocinera COME algo y recupera 12 HP.", ConsoleColor.DarkRed);
                return true;
            }
            return false; // los demas no tienen especial
        }

        // NUEVO: usar un objeto del inventario
        static bool UsarObjeto(ref Jaguar j)
        {
            Console.WriteLine();
            Escribir("  -- Mochila --", ConsoleColor.Cyan);
            for (int i = 0; i < itemNombres.Length; i++)
                Console.WriteLine("   [" + (i + 1) + "] " + itemNombres[i] + " x" + itemCantidad[i]);
            Console.WriteLine("   [0] Volver");
            Console.Write("  Elige: ");
            int sel = LeerEntero(0, itemNombres.Length);
            if (sel == 0) return false;

            int idx = sel - 1;
            if (itemCantidad[idx] <= 0)
            {
                Escribir("  No te quedan de ese objeto.", ConsoleColor.DarkYellow);
                return false;
            }
            itemCantidad[idx]--;

            if (idx == 0) // Refresco UAM: +40 HP
            {
                j.hp = Minimo(j.hp + 40, j.hpMax);
                Escribir("  Bebes un Refresco UAM. +40 HP", ConsoleColor.Green);
            }
            else if (idx == 1) // Cafe Cargado: +25 magia
            {
                RecuperarMagia(ref j, 25);
                Escribir("  Tomas un Cafe Cargado. +25 magia (hasta tu tope)", ConsoleColor.Green);
            }
            else // Empanada Magica: +30 HP y +15 magia
            {
                j.hp = Minimo(j.hp + 30, j.hpMax);
                RecuperarMagia(ref j, 15);
                Escribir("  Devoras una Empanada Magica. +30 HP y +15 magia", ConsoleColor.Green);
            }
            return true; // usar objeto SI gasta tu turno
        }

        // ====================================================================
        //  MODULO JUEGO: Flujo, navegacion y eventos
        // ====================================================================
        static void BucleJuego()
        {
            bool jugando = true;
            while (jugando && jaguar.hp > 0)
            {
                MostrarMenuPrincipal();
                int opcion = LeerEntero(1, 7);

                switch (opcion)
                {
                    case 1: ExplorarZona(); break;
                    case 2: MostrarStats(jaguar); Pausa(); break;
                    case 3: MostrarMapa(zonaActual); Pausa(); break;
                    case 4:
                        if (zonaActual > 0)
                        {
                            zonaActual -= 1;
                            Escribir("  Retrocedes a: " + zonas[zonaActual].nombre, ConsoleColor.Cyan);
                        }
                        else Escribir("  Ya estas en la entrada del campus.", ConsoleColor.DarkYellow);
                        Pausa();
                        break;
                    case 5: GuardarProgreso(jaguar, zonaActual, enemigos); Pausa(); break;
                    case 6: CargarProgreso(ref jaguar, ref zonaActual, ref enemigos); Pausa(); break;
                    case 7: Escribir("  Saliendo del juego... ¡Hasta pronto!", ConsoleColor.Cyan); jugando = false; break;
                }

                if (zonaActual >= 5) { MostrarVictoria(); jugando = false; }
            }
            if (jaguar.hp <= 0) MostrarGameOver();
        }

        static void ExplorarZona()
        {
            // Narrativa solo la primera vez
            if (zonas[zonaActual].visitada == false)
            {
                Console.Clear();
                Escribir("== " + zonas[zonaActual].nombre + " ==", ConsoleColor.Cyan);
                EscribirLento(zonas[zonaActual].historia);
                zonas[zonaActual].visitada = true;
                Pausa();
            }

            // NUEVO: evento aleatorio de exploracion (rompe la linealidad)
            EventoExploracion(ref jaguar);
            if (jaguar.hp <= 0) return;

            bool todosDerrotados = true;
            for (int i = 0; i < zonas[zonaActual].cantidadEnemigos; i++)
            {
                int idx = zonas[zonaActual].indiceEnemigos[i];
                if (idx < 0) continue;

                if (enemigos[idx].vivo == true)
                {
                    bool esJefe = (idx == 6);
                    bool gano = IniciarCombate(ref jaguar, ref enemigos[idx], esJefe);
                    if (jaguar.hp <= 0) return;
                    if (gano == false) { todosDerrotados = false; break; }
                }
            }

            if (todosDerrotados)
            {
                zonaActual += 1;
                Console.Clear();
                if (zonaActual < 5) Escribir("  ¡Zona liberada! Avanzas a: " + zonas[zonaActual].nombre, ConsoleColor.Green);
                else Escribir("  ¡Has liberado todo el campus de la UAM!", ConsoleColor.Green);
                Pausa();
            }
            else
            {
                Escribir("  Aun quedan enemigos en esta zona. No puedes avanzar.", ConsoleColor.DarkYellow);
                Pausa();
            }
        }

        // NUEVO: eventos aleatorios al explorar (incluye una decision)
        static void EventoExploracion(ref Jaguar j)
        {
            if (rng.Next(0, 100) < 45) return; // 45% no pasa nada

            Console.WriteLine();
            int tipo = rng.Next(0, 5);

            if (tipo == 0) // Hallar objeto
            {
                int it = rng.Next(0, 3);
                itemCantidad[it]++;
                Escribir("  Encuentras algo util: 1x " + itemNombres[it], ConsoleColor.Cyan);
            }
            else if (tipo == 1) // Trampa
            {
                int dano = rng.Next(8, 17);
                j.hp -= dano;
                if (j.hp < 1) j.hp = 1; // una trampa no te mata
                Escribir("  ¡Te resbalas en el piso recien trapeado! -" + dano + " HP", ConsoleColor.DarkRed);
            }
            else if (tipo == 2) // Fuente del campus
            {
                j.hp = Minimo(j.hp + 20, j.hpMax);
                RecuperarMagia(ref j, 10);
                Escribir("  Bebes de la fuente de la UAM. +20 HP y +10 magia", ConsoleColor.Green);
            }
            else if (tipo == 3) // Apuntes energizantes
            {
                RecuperarMagia(ref j, 15);
                Escribir("  Hallas unos apuntes que te energizan. +15 magia", ConsoleColor.Green);
            }
            else // Decision: maquina expendedora atascada
            {
                Escribir("  Hay una maquina expendedora atascada con una empanada dentro.", ConsoleColor.Yellow);
                Console.WriteLine("   [1] Forzarla (riesgo)   [2] Dejarla");
                Console.Write("  ¿Que haces? ");
                int op = LeerEntero(1, 2);
                if (op == 1)
                {
                    if (rng.Next(0, 100) < 55)
                    {
                        itemCantidad[2]++;
                        Escribir("  ¡Cae la empanada! Obtienes 1x Empanada Magica.", ConsoleColor.Green);
                    }
                    else
                    {
                        int dano = rng.Next(6, 13);
                        j.hp -= dano;
                        if (j.hp < 1) j.hp = 1;
                        Escribir("  La maquina se te cae encima. -" + dano + " HP", ConsoleColor.DarkRed);
                    }
                }
                else Escribir("  Sigues de largo, prudente.", ConsoleColor.Gray);
            }
            Pausa();
        }

        static void MostrarStats(Jaguar j)
        {
            Console.Clear();
            Escribir("===== STATS DEL JAGUAR =====", ConsoleColor.Cyan);
            Console.WriteLine("  Nombre   : " + j.nombre);
            Console.WriteLine("  Nivel    : " + j.nivel);
            Console.WriteLine("  EXP      : " + j.exp + " / " + (j.nivel * 50));
            Console.WriteLine("  Ataque   : " + j.ataque);
            Console.WriteLine("  Defensa  : " + j.defensa);
            Console.WriteLine("  Magia    : " + j.magia + " / " + MagiaMax(j));
            Console.Write("  ");
            MostrarBarraHP(j.hp, j.hpMax);
            Console.WriteLine("  Habilidades:");
            for (int i = 0; i < 4; i++)
            {
                if (j.habilidades[i] != null && j.habilidades[i] != "")
                    Console.WriteLine("    - " + j.habilidades[i]);
                else
                    Console.WriteLine("    - [Bloqueada]");
            }
            Console.WriteLine("  Mochila:");
            for (int i = 0; i < itemNombres.Length; i++)
                Console.WriteLine("    - " + itemNombres[i] + " x" + itemCantidad[i]);
        }

        static void MostrarMapa(int actual)
        {
            Console.Clear();
            Escribir("===== MAPA DEL CAMPUS UAM =====", ConsoleColor.Cyan);
            Console.WriteLine();
            for (int i = 0; i < 5; i++)
            {
                string marca;
                if (i == actual) marca = " <== ESTAS AQUI";
                else if (zonas[i].visitada) marca = " (liberada)";
                else marca = " (bloqueada)";
                Console.WriteLine("  [" + (i + 1) + "] " + zonas[i].nombre + marca);
                if (i < 4) Console.WriteLine("   |");
            }
        }

        // ====================================================================
        //  MODULO PERSISTENCIA: System.IO (linea a linea, sin cabeceras)
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
            lineas[16] = itemCantidad[0].ToString(); // NUEVO
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