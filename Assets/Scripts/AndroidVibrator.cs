using UnityEngine;

public class AndroidVibrator : MonoBehaviour
{
    /// <summary>
    /// Vibrates the device for the given duration and amplitude.
    /// </summary>
    /// <param name="milliseconds">Duration in milliseconds (e.g., 500ms).</param>
    /// <param name="amplitude">Amplitude from 1 (weak) to 255 (strong).</param>
    public void Vibrate(int milliseconds = 500, int amplitude = 255)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
                AndroidJavaObject vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator");

                if (vibrator != null)
                {
                    AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION");
                    int sdkInt = version.GetStatic<int>("SDK_INT");

                    if (sdkInt >= 26)
                    {
                        // Use VibrationEffect with amplitude
                        AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                        AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                            "createOneShot", milliseconds, amplitude);
                        vibrator.Call("vibrate", effect);
                    }
                    else
                    {
                        // Fallback: simple vibration (no amplitude control)
                        vibrator.Call("vibrate", (long)milliseconds);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Vibration failed: " + ex.Message);
        }
#else
        Debug.Log("Vibration attempted in Editor or unsupported platform.");
#endif
    }
}
