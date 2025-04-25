using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;

public static class ExtensionMethods
{
    /// <summary>
    /// Remaps a value from one range to another.
    /// </summary>
    /// <param name="value">The input value to remap.</param>
    /// <param name="originalMin">The minimum value of the original range.</param>
    /// <param name="originalMax">The maximum value of the original range.</param>
    /// <param name="targetMin">The minimum value of the target range.</param>
    /// <param name="targetMax">The maximum value of the target range.</param>
    /// <returns>The remapped value within the target range.</returns>

    public static IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, bool fadeIn, float duration)
    {
       
        float targetAlpha = fadeIn ? 1f : 0f;
        float initialAlpha = fadeIn ? 0f : 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
    public static  IEnumerator FadeTextIn(TextMeshProUGUI text, float duration)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsed / duration);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
    }

    public static double Remap(double value, double from1, double to1, double from2, double to2)
    {
        if (to1 - from1 == 0) return from2; // Avoid division by zero if input range is zero
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static double ConvertDistanceToTime(double distance, double distanceStart, double distanceEnd, double timeStart, double timeEnd)
    {
        distance = Math.Clamp(distance, distanceStart, distanceEnd);

        if (distanceStart <= 0 || distance <= 0)
        {
            if (distanceStart <= 0) return timeStart;
            if (distance == distanceStart) return timeStart;
        }
        if (distanceEnd == distanceStart)
        {
            return timeStart;
        }

        if (distanceEnd <= distanceStart)
        {
            Debug.LogWarning("ConvertDistanceToTime: distanceEnd must be greater than distanceStart for logarithmic interpolation.");
            return timeStart;
        }

        double logDist = Math.Log(distance);
        double logStart = Math.Log(distanceStart);
        double logEnd = Math.Log(distanceEnd);

        if (logEnd == logStart) return timeStart;

        double normalizedDistance = (logDist - logStart) / (logEnd - logStart);

        normalizedDistance = Math.Clamp(normalizedDistance, 0.0, 1.0);

        return timeStart + (timeEnd - timeStart) * normalizedDistance;
    }
}
