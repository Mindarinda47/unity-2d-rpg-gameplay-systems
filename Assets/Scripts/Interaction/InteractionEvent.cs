using System;

public static class InteractionEvents
{
    public static event Action<string> NpcTalkCompleted;

    public static void RaiseNpcTalkCompleted(string npcId)
    {
        NpcTalkCompleted?.Invoke(npcId);
    }
}