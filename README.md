# Jaguarcito UAM – RPG del Campus

Videojuego de rol (RPG) por turnos desarrollado en **C#** para consola, como proyecto final de la asignatura **Introducción a la Programación** de la **Universidad Americana (UAM)**.

---

## Descripción general del sistema

**Jaguarcito UAM** es un RPG por turnos ambientado en el campus de la UAM. El jugador controla al **Jaguar**, mascota oficial de la universidad, que debe recorrer cinco zonas del campus —desde la Entrada Principal hasta la Biblioteca— derrotando a enemigos inspirados en la vida universitaria (el Profe de Cálculo, el Examen Sorpresa, la Cocinera Enojada, entre otros) hasta enfrentarse al jefe final: **El Director en la Sombra**.

El juego integra navegación por zonas, sistema de combate por turnos, progreso del personaje (niveles y habilidades), un inventario de objetos, eventos aleatorios de exploración y guardado de partida en archivo de texto. Todo dentro del entorno de consola.

El proyecto está construido con **programación estructurada**: la lógica se organiza mediante `struct`, arreglos de tamaño fijo, métodos estáticos, ciclos, condicionales, validación de entrada y manejo de archivos. **No se utilizan clases (fuera de la contenedora `Program`), herencia ni `List<T>`.**

### Características principales

- Mapa navegable de 5 zonas que se desbloquean progresivamente (no se avanza sin derrotar a todos los enemigos de la zona).
- Combate por turnos con economía de maná: el ataque básico recupera maná y las habilidades lo consumen.
- Sistema de niveles y experiencia: el Jaguar sube de nivel, mejora sus stats y desbloquea habilidades con efectos distintos.
- Inventario de objetos (Refresco UAM, Café Cargado, Empanada Mágica) que se obtienen explorando y combatiendo.
- Eventos aleatorios al explorar (trampas, fuentes que curan, decisiones).
- IA enemiga variada y un jefe final con fases.
- Guardado y carga de progreso en archivo de texto (`progreso.txt`) y ranking de partidas (`ranking.txt`).
- Interfaz de consola con texto coloreado, barras de HP en ASCII y narrativa animada.

---

## Requisitos previos

- **.NET SDK 10.0** (incluido con Visual Studio 2026).
- **Visual Studio 2026** con la carga de trabajo *Desarrollo de escritorio de .NET*, **o** el SDK de .NET para usar la línea de comandos.
- Sistema operativo Windows recomendado (la consola usa colores y caracteres ASCII extendidos).

---

## Instrucciones de instalación y ejecución

### Opción A — Visual Studio 2026 (recomendada)

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/USUARIO/jaguarcito-uam.git
   ```
2. Abrir Visual Studio 2026.
3. Seleccionar **Abrir un proyecto o una solución** y elegir el archivo `.sln` del repositorio (o abrir la carpeta del proyecto).
4. Asegurarse de que el proyecto compile con .NET 10.0.
5. Ejecutar con **Depurar ▶ Iniciar sin depurar** (o presionar `Ctrl + F5`).

### Opción B — Línea de comandos (.NET CLI)

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/USUARIO/jaguarcito-uam.git
   cd jaguarcito-uam
   ```
2. Compilar y ejecutar:
   ```bash
   dotnet run
   ```

> Los archivos `progreso.txt` y `ranking.txt` se generan automáticamente en la carpeta de ejecución la primera vez que se guarda una partida. Si no existe `progreso.txt`, el juego inicia una partida nueva sin error.

---

## Cómo se juega

Desde el menú principal el jugador puede explorar la zona actual, ver las estadísticas del Jaguar, consultar el mapa del campus, volver a una zona anterior, guardar, cargar o salir. Durante el combate se elige entre Zarpazo (básico), Habilidad especial, Curar, Usar objeto y Huir. La estrategia consiste en tejer ataques básicos para sostener el maná y usar las habilidades en el momento adecuado, gestionando el HP con curaciones y objetos.

---

## Estructura del proyecto

```
JaguarUAM/
│
├── README.md                  # Este archivo
│
└── JaguarUAM/                 # Carpeta de la solución
    │
    ├── JaguarUAM/             # Proyecto principal
    │   ├── Program.cs         # Código fuente completo del juego
    │   └── JaguarUAM.csproj   # Archivo de configuración del proyecto .NET
    │
    └── JaguarUAM.sln          # Solución de Visual Studio
```

### Organización interna de `Program.cs`

El código está dividido en módulos lógicos (conjuntos de métodos estáticos agrupados por responsabilidad):

| Módulo | Responsabilidad | Métodos principales |
|--------|-----------------|---------------------|
| **Structs** | Tipos de datos | `Jaguar`, `Enemigo`, `Zona` |
| **Datos** | Inicialización | `InicializarEnemigos()`, `InicializarZonas()`, `InicializarJaguar()` |
| **Personaje** | Progreso del Jaguar | `GanarExp()`, `SubirNivel()`, `MagiaMax()` |
| **Combate** | Lógica de pelea | `IniciarCombate()`, `EjecutarHabilidad()`, `UsarObjeto()`, `AccionEspecialEnemigo()` |
| **Juego** | Flujo y navegación | `BucleJuego()`, `ExplorarZona()`, `EventoExploracion()`, `MostrarStats()`, `MostrarMapa()` |
| **Persistencia** | Archivos (System.IO) | `GuardarProgreso()`, `CargarProgreso()`, `GuardarRanking()` |
| **UI** | Presentación en consola | `Escribir()`, `EscribirLento()`, `MostrarBarraHP()`, `MostrarMenuPrincipal()` |
| **Utilidades** | Apoyo | `LeerEntero()`, `Maximo()`, `Minimo()`, `Pausa()` |

Los datos del juego se almacenan en arreglos globales de tamaño fijo: `Enemigo[7]` (los siete enemigos) y `Zona[5]` (las cinco zonas), más arreglos paralelos para el inventario.

---

## Integrantes

- Sergio Cabrera
- Jonas Hodgson
- Ricardo Hamad
- Ervin Rocha

**Docente:** Jose Duran
**Universidad Americana** · Introducción a la Programación · Junio 2026
