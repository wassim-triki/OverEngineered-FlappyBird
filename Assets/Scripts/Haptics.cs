using CandyCoded.HapticFeedback;
using UnityEngine;
public static class Haptics
{
    
    // Prevent spamming the motor (rapid multi-collider hits)
    public static float MinInterval = 0.06f; // seconds
    static float _last;

    static bool Cooldown()
    {
        float now = Time.realtimeSinceStartup;
        if (now - _last < MinInterval) return true;
        _last = now; return false;
    }

    public static void Light()
    {
        if (Cooldown()) return;
        HapticFeedback.LightFeedback();
    }

    public static void Medium()
    {
        if (Cooldown()) return;
        HapticFeedback.MediumFeedback();
    }

    public static void Heavy()
    {
        if (Cooldown()) return;
        HapticFeedback.HeavyFeedback();
    }

}
