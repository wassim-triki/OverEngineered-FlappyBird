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
        OneShot(30, 64);
    }

    public static void Medium()
    {
        if (Cooldown()) return;
        OneShot(50, 128);
    }

    public static void Heavy()
    {
        if (Cooldown()) return;
        OneShot(80, 255);
    }

    public static void OneShot(long durationMs, int amplitude = 255)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
            {
                if (vibrator == null || !vibrator.Call<bool>("hasVibrator")) return;

                int sdk = new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT");
                if (sdk >= 26)
                {
                    using (var vibEffect = new AndroidJavaClass("android.os.VibrationEffect"))
                    {
                        // Clamp 1..255 for safety
                        int amp = Mathf.Clamp(amplitude, 1, 255);
                        var effect = vibEffect.CallStatic<AndroidJavaObject>("createOneShot", durationMs, amp);
                        vibrator.Call("vibrate", effect);
                    }
                }
                else
                {
                    vibrator.Call("vibrate", durationMs); // legacy
                }
            }
        }
        catch { /* ignore if device blocks vibration */ }
#else
        // iOS / Editor / other: basic fallback (no intensity control)
        Handheld.Vibrate();
#endif
    }

    public static void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
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
