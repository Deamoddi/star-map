using star_map.Models;

namespace star_map.Display;

/// <summary>
/// Service for displaying Starlink data in console
/// </summary>
public class ConsoleDisplay
{
    /// <summary>
    /// Displays application header
    /// </summary>
    public void DisplayHeader()
    {
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║           StarMap by Maestro           ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();
    }

    /// <summary>
    /// Displays configuration information
    /// </summary>
    public void DisplayConfiguration(string endpoint)
    {
        Console.WriteLine($"📡 Конфігурація");
        Console.WriteLine($"   Starlink gRPC IP: {endpoint}");
        Console.WriteLine();
    }

    /// <summary>
    /// Displays status message
    /// </summary>
    public void DisplayStatus(string message)
    {
        Console.WriteLine($"⚙️  {message}");
    }

    /// <summary>
    /// Displays success message
    /// </summary>
    public void DisplaySuccess(string message)
    {
        Console.WriteLine($"✅ {message}");
        Console.WriteLine();
    }

    /// <summary>
    /// Displays location data
    /// </summary>
    public void DisplayLocation(LocationData location)
    {
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║       📍 GPS дані з тарілки            ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();

        Console.WriteLine($"🌍 Координати:");
        Console.WriteLine($"   Широта:        {location.Latitude:F6}°");
        Console.WriteLine($"   Довгота:       {location.Longitude:F6}°");
        Console.WriteLine($"   Висота:        {location.Altitude:F2} m");
        Console.WriteLine($"   MGRS:            {location.Mgrs}");
        Console.WriteLine();

        Console.WriteLine($"📊 Інформація про позиціонування:");
        Console.WriteLine($"   Джерело:          {location.Source}");
        Console.WriteLine($"   Точність:        {location.AccuracyMeters:F2} м");
        Console.WriteLine($"   Горизонтальна швидкість: {location.HorizontalSpeedMps:F2} м/с");
        Console.WriteLine($"   Вертикальна швидкість:  {location.VerticalSpeedMps:F2} м/с");
        Console.WriteLine();
    }

    /// <summary>
    /// Displays dish status including azimuth and elevation
    /// </summary>
    public void DisplayDishStatus(DishStatus status)
    {
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║         📡 Статус тарілки              ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();
        
        Console.WriteLine($"🔧 Інфа про залізяку:");
        Console.WriteLine($"   Device ID:            {status.DeviceId}");
        Console.WriteLine($"   Версія заліза:        {status.HardwareVersion}");
        Console.WriteLine($"   Версія ПЗ:            {status.SoftwareVersion}");
        Console.WriteLine($"   Аптайм:               {FormatUptime(status.UptimeSeconds)}");
        Console.WriteLine();

        Console.WriteLine($"🎯 Dish Orientation (Azimuth & Elevation):");
        Console.WriteLine($"   Current Azimuth:    {status.BoresightAzimuthDeg:F2}°");
        Console.WriteLine($"   Current Elevation:  {status.BoresightElevationDeg:F2}°");
        Console.WriteLine($"   Desired Azimuth:    {status.DesiredBoresightAzimuthDeg:F2}°");
        Console.WriteLine($"   Desired Elevation:  {status.DesiredBoresightElevationDeg:F2}°");
        Console.WriteLine();
        
        DisplayAlignmentStatus(status);
    }

    /// <summary>
    /// Displays complete Starlink data
    /// </summary>
    public void DisplayCompleteData(StarlinkData data)
    {
        if (data.Location != null)
        {
            DisplayLocation(data.Location);
        }

        if (data.Status != null)
        {
            DisplayDishStatus(data.Status);
        }

        Console.WriteLine($"🔗 Підключено до: {data.Endpoint}");
        Console.WriteLine();
    }

    /// <summary>
    /// Displays error message
    /// </summary>
    public void DisplayError(string message, Exception? ex = null)
    {
        Console.WriteLine($"\n❌ Алярм!: {message}");
        
        if (ex != null)
        {
            Console.WriteLine($"Детально: {ex.Message}");
            
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Ось це: {ex.InnerException.Message}");
            }
        }
    }

    /// <summary>
    /// Displays waiting prompt
    /// </summary>
    public void DisplayWaitPrompt()
    {
        Console.WriteLine("Тицьни щось щоб вийти");
    }

    private void DisplayAlignmentStatus(DishStatus status)
    {
        var azimuthDiff = Math.Abs(status.BoresightAzimuthDeg - status.DesiredBoresightAzimuthDeg);
        var elevationDiff = Math.Abs(status.BoresightElevationDeg - status.DesiredBoresightElevationDeg);
        
        var alignmentQuality = (azimuthDiff < 1.0 && elevationDiff < 1.0) ? "Excellent" :
                              (azimuthDiff < 5.0 && elevationDiff < 5.0) ? "Good" :
                              (azimuthDiff < 10.0 && elevationDiff < 10.0) ? "Fair" : "Poor";
        
        var emoji = alignmentQuality == "Excellent" ? "✅" :
                   alignmentQuality == "Good" ? "👍" :
                   alignmentQuality == "Fair" ? "⚠️" : "❌";
        
        Console.WriteLine($"   Якість вирівнювання:    {emoji} {alignmentQuality}");
        Console.WriteLine($"   Зміщення по азимуту:    {azimuthDiff:F2}°");
        Console.WriteLine($"   Зміщення по тангажу:    {elevationDiff:F2}°");
        Console.WriteLine();
    }

    private string FormatUptime(ulong seconds)
    {
        var timeSpan = TimeSpan.FromSeconds(seconds);
        
        if (timeSpan.TotalDays >= 1)
            return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h {timeSpan.Minutes}m";
        else if (timeSpan.TotalHours >= 1)
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
        else if (timeSpan.TotalMinutes >= 1)
            return $"{(int)timeSpan.TotalMinutes}m {timeSpan.Seconds}s";
        else
            return $"{timeSpan.Seconds}s";
    }
}
