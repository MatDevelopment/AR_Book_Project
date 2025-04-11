using System;
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
