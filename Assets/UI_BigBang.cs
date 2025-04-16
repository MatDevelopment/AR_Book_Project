using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BigBang : MonoBehaviour
{
    public GameObject scanEnvironmentPanel, BigBangInformationPanel;
    public double DistanceTotime = 0;
    public double RemappedValue = 0;

    [SerializeField]
    private Slider Slider_Time;
    [SerializeField]
    private TextMeshProUGUI Text_Time, text_Information; 

    void Awake()
    {
        // controlTimeWithDistance = GetComponent<ControlTimeWithDistance>();
        scanEnvironmentPanel.SetActive(true);
        BigBangInformationPanel.SetActive(false);
    }

    public void HideStartText()
    {
        scanEnvironmentPanel.SetActive(false);
    }

    public void ShowBigBangInformation()
    {
        BigBangInformationPanel.SetActive(true);
    }
    public void UpdateTimeText(double currentDistance)
    {
        string timeText = FormatTimeFromDistance(currentDistance);
        Text_Time.text = timeText;
    }
    public void UpdateTimeSlider(float input)
    {
        double remappedValue = ExtensionMethods.Remap(input, 0, 10, 0.0, 1000f);
        Slider_Time.value = (float)remappedValue;
        RemappedValue = remappedValue;
    }

    public void SetInformationText(float inputtime)
    {
        string information = "";
        if (inputtime <= 0.1f)
        {
            information = "0 - Alt stof i universet er samlet i et enkelt punkt.";
        }
        else if (inputtime > 0.1f && inputtime <= 2.1f)
        {
            information = "1 nanosekund - Universet er en ekstremt varm supper af elemæntar partikler såsom Kvarker og Elektroner";
        }

        else if (inputtime > 2.5f && inputtime <= 4.2f)
        {
            information = "1 sekund - Atomkerner til brint og helium dannes";
        }
        else if (inputtime > 5f && inputtime <= 5.6f)
        {
            information = "377 tusind år - Atomkerner kombineres med elektroner og hydrogen atomer dannes";
        }
        else if (inputtime > 6f && inputtime <= 6.6f)
        {
            information = "300-500 millioner år - Tyngdekraften presser hydrogen atomer sammen til de første kæmpe stjerner";
        }
        else if (inputtime > 6.8f && inputtime <= 7.4f)
        {
            information = "1 millard år - De første stjerner er så massive at de eksploderer og skyder tungere elementer ud i alle retninger";
        }
        else if (inputtime > 7.8f && inputtime <= 8.8f)
        {
            information = "9.7 millarder år - Vores lille solsystem samles af stof fra de tidligere super novaer";
        }
        else if (inputtime > 9.9f)
        {
            information = "Nu";
        }
        else
            information = "";
            text_Information.text = information;
    }

    // Conversion constant: seconds in one year (using 365.25 days with 86400 s/day is a rough approximation)
    private const double SecondsPerYear = 31557600;

    public string FormatTimeFromDistance(double distance)
    {
        // If the distance is zero or less, show zero (with the smallest unit)
        if (distance <= 0)
            return "0 nanoseconds";

        // For very small distances (between 0 and 0.6) linearly interpolate up to 1 nanosecond.
        if (distance <= 0.6)
        {
            double fraction = distance / 0.6;
            // Time in seconds from 0 to 1e-9 seconds, then convert to years.
            double timeSec = fraction * 1e-9;
            double timeYears = timeSec / SecondsPerYear;
            return FormatTime(timeYears);
        }

        // Define breakpoints for distance and corresponding time in years.
        // Note: 1 nanosecond and 1 second are converted into years.
        double[] dPoints = { 0.6, 2.5, 5.0, 6.0, 6.8, 7.8, 9.9, 10.0 };
        double[] tPointsYears = new double[dPoints.Length];
        tPointsYears[0] = 1e-9 / SecondsPerYear;  // 1 nanosecond
        tPointsYears[1] = 1.0 / SecondsPerYear;     // 1 second
        tPointsYears[2] = 377000;                    // 377 thousand years
        tPointsYears[3] = 4e8;                       // 400 million years (a median for 300–500)
        tPointsYears[4] = 1e9;                       // 1 billion years
        tPointsYears[5] = 9.7e9;                     // 9.7 billion years
        tPointsYears[6] = 13.7e9;                    // 13.7 billion years
        tPointsYears[7] = 13.7e9;                    // capped at maximum time

        // Clamp above the highest breakpoint.
        if (distance >= dPoints[dPoints.Length - 1])
        {
            return FormatTime(tPointsYears[dPoints.Length - 1]);
        }

        // Find the segment in which the current distance lies.
        int seg = 0;
        for (int i = 0; i < dPoints.Length - 1; i++)
        {
            if (distance >= dPoints[i] && distance <= dPoints[i + 1])
            {
                seg = i;
                break;
            }
        }

        // Interpolate between the two breakpoints using a logarithmic interpolation
        // (except in cases where one of the values is zero).
        double fractionSegment = (distance - dPoints[seg]) / (dPoints[seg + 1] - dPoints[seg]);
        double t0 = tPointsYears[seg];
        double t1 = tPointsYears[seg + 1];
        double timeYearsInterpolated;
        if (t0 > 0 && t1 > 0)
        {
            double logT0 = Math.Log(t0);
            double logT1 = Math.Log(t1);
            double logT = logT0 + (logT1 - logT0) * fractionSegment;
            timeYearsInterpolated = Math.Exp(logT);
        }
        else
        {
            // Fallback to linear interpolation if necessary.
            timeYearsInterpolated = t0 + (t1 - t0) * fractionSegment;
        }

        return FormatTime(timeYearsInterpolated);
    }

    /// <summary>
    /// Converts a time (in years) to a formatted string with at most two decimals,
    /// selecting an appropriate unit: nanoseconds, sekunder (seconds), tusind år (thousand years),
    /// millioner år (million years) or milliarder år (billion years).
    /// </summary>
    private string FormatTime(double timeYears)
    {
        // Convert the time in years into seconds.
        double timeSec = timeYears * SecondsPerYear;

        if (timeSec < 1)
        {
            double ns = timeSec * 1e9; // convert seconds to nanoseconds
            return $"{TruncateToTwoDigits(ns)} nanoseconds";
        }
        else if (timeSec < 60)
        {
            return $"{TruncateToTwoDigits(timeSec)} sekunder";
        }
        else if (timeSec < 3600) // Ny gren: hvis tiden er under én time, konverter til minutter.
        {
            double minutes = timeSec / 60;
            return $"{TruncateToTwoDigits(minutes)} minutter";
        }
        else if (timeYears < 1e6)
        {
            double value = timeYears / 1e3;
            return $"{TruncateToTwoDigits(value)} tusind år";
        }
        else if (timeYears < 1e9)
        {
            double value = timeYears / 1e6;
            return $"{TruncateToTwoDigits(value)} millioner år";
        }
        else
        {
            double value = timeYears / 1e9;
            return $"{TruncateToTwoDigits(value)} milliarder år";
        }
    }


    // Helper to reduce the number to two digits before the decimal
    private string TruncateToTwoDigits(double value)
    {
        while (value >= 100)
        {
            value /= 10;
        }
        return value.ToString("F2");
    }


    /// <summary>
    /// Updates the UI text element to display the time corresponding to the current distance.
    /// Pass in the current distance value (typically between distanceStart and distanceEnd).
    /// </summary>

}