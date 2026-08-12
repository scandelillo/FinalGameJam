/// <summary>
/// Fuente única de verdad para los IDs de zona. Tanto WaveSpawner como los
/// scripts de puertas deben usar este enum en vez de escribir el nombre a mano,
/// así el compilador avisa si hay un typo en vez de fallar en silencio.
/// </summary>
public enum ZoneId
{
    HabitacionInicial,
    ZonaB,
    ZonaC
    // agreguen aquí cada nueva zona que diseñen
}
