using UnityEngine;

/// <summary>
/// Sets the application's target framerate at startup for all builds.
/// Placed outside any scene using a runtime initialize hook so it always runs first.
/// </summary>
public static class FrameRateInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void SetTargetFrameRate()
    {
        // Disable vSync so Application.targetFrameRate takes effect.
        // If you prefer vSync control via Quality settings, remove this line.
        QualitySettings.vSyncCount = 0;

        // Choose a target suitable for your platforms. 60 is a safe default.
        // You can branch per platform if desired.
        // Example:
        // #if UNITY_ANDROID || UNITY_IOS
        //     Application.targetFrameRate = 60; // or 120 on devices that support it
        // #else
        //     Application.targetFrameRate = 60;
        // #endif
        Application.targetFrameRate = 60;

        // Optional: align physics step to target framerate (usually unnecessary).
        Time.fixedDeltaTime = 1f / 60f;
    }
}

