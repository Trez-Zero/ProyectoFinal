// ============================================================================
//  ERVIN ROCHA - MODULO EXPLORACION Y FLUJO DEL JUEGO
//  Contiene bucle principal, exploracion de zonas y eventos aleatorios.
//  NOTA: Este codigo es una parte del Program.cs final.
// ============================================================================

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

