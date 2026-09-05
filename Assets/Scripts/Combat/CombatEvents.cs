using System;

public static class CombatEvents
{
    public static event Action<string> EnemyKilled;

    public static void RaiseEnemyKilled(string enemyId)
    {
        EnemyKilled?.Invoke(enemyId);
    }
}