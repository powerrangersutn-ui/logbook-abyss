using System;

//Es el Conector. Contiene eventos y permite que todos los anteriores no tengan que conocerse entre sí.
public static class UIGameEvents
{
    public static Action<int, int> onPlayerHealthChanged;
    public static Action onPlayerDeath;

    //Llamada
    public static void OnPlayerHealthChanged(int currentHealth, int maxHealth)
    {
        onPlayerHealthChanged.Invoke(currentHealth, maxHealth);
    }

    public static void OnPlayerDeath()
    {
        onPlayerDeath?.Invoke();
    }
}
