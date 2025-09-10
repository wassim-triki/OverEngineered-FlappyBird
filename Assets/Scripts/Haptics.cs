using UnityEngine;

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
using CandyCoded.HapticFeedback; // only available on mobile builds
#endif

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
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        HapticFeedback.LightFeedback();
#else
        // WebGL / PC / Editor: no-op
#endif
    }

    public static void Medium()
    {
        if (Cooldown()) return;
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        HapticFeedback.MediumFeedback();
#else
        // WebGL / PC / Editor: no-op
#endif
    }

    public static void Heavy()
    {
        if (Cooldown()) return;
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        HapticFeedback.HeavyFeedback();
#else
        // WebGL / PC / Editor: no-op
#endif
    }

    public static void Cancel()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
            {
                vibrator?.Call("cancel");
            }
        }
        catch { }
#endif
    }
}