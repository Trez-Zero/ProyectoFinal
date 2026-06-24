// ============================================================================
//  RICARDO HAMAD - MODULO COMBATE, HABILIDADES E INVENTARIO
//  Contiene combate, habilidades, objetos, IA enemiga y jefe final.
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

