using UnityEngine;

public class AndroidVibrator : MonoBehaviour
{
    private bool isVibrating = false;

#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject vibrator;
    private int sdkInt;

    void Awake()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
            vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator");

            AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION");
            sdkInt = version.GetStatic<int>("SDK_INT");
        }
    }
#endif

    /// <summary>
    /// Vibrates once for a given duration and amplitude.
    /// </summary>
    public void Vibrate(int milliseconds = 500, int amplitude = 255)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (vibrator == null) return;

        if (sdkInt >= 26)
        {
            AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                "createOneShot", milliseconds, amplitude);
            vibrator.Call("vibrate", effect);
        }
        else
        {
            vibrator.Call("vibrate", (long)milliseconds);
        }
#else
        Debug.Log("Vibration (once) called — Editor mode or unsupported.");
#endif
    }

    /// <summary>
    /// Starts or stops a repeating vibration.
    /// </summary>
    /// <param name="duration">Duration of each pulse in ms.</param>
    /// <param name="pause">Pause between pulses in ms.</param>
    /// <param name="amplitude">Strength (1–255).</param>
    public void ToggleContinuousVibration(int duration = 300, int pause = 200, int amplitude = 180)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (vibrator == null) return;

        if (isVibrating)
        {
            vibrator.Call("cancel");
            isVibrating = false;
        }
        else
        {
            long[] pattern = { 0, duration, pause };
            int repeatIndex = 0; // Repeat from beginning

            if (sdkInt >= 26)
            {
                int[] amplitudes = { 0, amplitude, 0 };
                AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                    "createWaveform", pattern, amplitudes, repeatIndex);
                vibrator.Call("vibrate", effect);
            }
            else
            {
                vibrator.Call("vibrate", pattern, repeatIndex);
            }

            isVibrating = true;
        }
#else
        Debug.Log("Toggle vibration called — Editor mode or unsupported.");
#endif
    }
}
