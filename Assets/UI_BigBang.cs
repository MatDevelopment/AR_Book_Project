using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BigBang : MonoBehaviour
{
  
    public double DistanceTotime = 0;
    public double RemappedValue = 0;

    [SerializeField]
    private Slider Slider_Time;
    [SerializeField]
    private TextMeshProUGUI Text_Time; 


    void Start()
    {


        // controlTimeWithDistance = GetComponent<ControlTimeWithDistance>();

    }

    public void UpdateTimeSliderAndText(double distance, double distanceStart, double distanceEnd, double timeStart, double timeEnd)
    {
        double currentYears = ConvertDistanceToTime(distance, distanceStart, distanceEnd, timeStart, timeEnd);

        double remappedValue = ExtensionMethods.Remap(currentYears, timeStart, timeEnd, 0.0, 1000f);
        Slider_Time.value = (float)remappedValue; 
        RemappedValue = remappedValue; 

        Text_Time.text = FormatTimeFromYears(currentYears);
    }

    private double ConvertDistanceToTime(double distance, double distanceStart, double distanceEnd, double timeStart, double timeEnd)
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


        DistanceTotime = timeStart + (timeEnd - timeStart) * normalizedDistance;

        return DistanceTotime;
    }


    private string FormatTimeFromYears(double years)
    {
        if (years < 0)
        {

            throw new ArgumentOutOfRangeException(nameof(years), "Input �r kan ikke v�re negativt.");
         
        }

        // Konstanter for konvertering (bruger double for pr�cision)
        const double DaysPerYear = 365.25; // Gennemsnit inkl. skud�r
        const double HoursPerDay = 24.0;
        const double MinutesPerHour = 60.0;
        const double SecondsPerMinute = 60.0;

        // Beregn gr�nserne i �r
        const double MinYearsForDays = 1.0 / DaysPerYear;               // Gr�nse for at vise dage i stedet for timer
        const double MinYearsForHours = MinYearsForDays / HoursPerDay;      // Gr�nse for at vise timer i stedet for minutter
        const double MinYearsForMinutes = MinYearsForHours / MinutesPerHour;  // Gr�nse for at vise minutter i stedet for sekunder
        const double MinYearsForSeconds = MinYearsForMinutes / SecondsPerMinute; // Teoretisk gr�nse for sekunder (meget lille)

        // Formateringsstreng for 2 decimaler. Bruger InvariantCulture for at sikre "." som decimaltegn.
        // Hvis du foretr�kker "," baseret p� systemets kultur, kan du fjerne CultureInfo.InvariantCulture.
        string format = "F2";
        CultureInfo culture = CultureInfo.InvariantCulture; // Brug "." som decimaltegn

        // Tjek fra st�rste enhed til mindste
      
        if (years >= 1000000)
        {
            return $"{years.ToString(format, culture)} millioner �r";
        }  
        else if (years >= 1.0)
        {
            return $"{years.ToString(format, culture)} �r";
        }
        else if (years >= MinYearsForDays)
        {
            double totalDays = years * DaysPerYear;
            return $"{totalDays.ToString(format, culture)} dage";
        }
        else if (years >= MinYearsForHours)
        {
            double totalHours = years * DaysPerYear * HoursPerDay;
            return $"{totalHours.ToString(format, culture)} timer";
        }
        else if (years >= MinYearsForMinutes)
        {
            double totalMinutes = years * DaysPerYear * HoursPerDay * MinutesPerHour;
            return $"{totalMinutes.ToString(format, culture)} minutter";
        }
        else
        {
            // Hvis mindre end et minut, vis sekunder
            // H�ndterer ogs� 0 og meget sm� positive v�rdier
            double totalSeconds = years * DaysPerYear * HoursPerDay * MinutesPerHour * SecondsPerMinute;

            // Lille korrektion for potentielle flydende komma-un�jagtigheder n�r 0
            if (Math.Abs(totalSeconds) < 1e-9)
            {
                totalSeconds = 0;
            }
            return $"{totalSeconds.ToString(format, culture)} sekunder";
        }
    }

   

}