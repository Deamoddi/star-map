using SpaceX.API.Device;
using CoordinateSharp;

namespace star_map.Models;

/// <summary>
/// Represents location data from Starlink device
/// </summary>
public class LocationData
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Altitude { get; set; }
    public PositionSource Source { get; set; }
    public double AccuracyMeters { get; set; }
    public double HorizontalSpeedMps { get; set; }
    public double VerticalSpeedMps { get; set; }
    public string Mgrs { get; set; } = string.Empty;

    public static LocationData FromResponse(GetLocationResponse response)
    {
        var lat = response.Lla?.Lat ?? 0;
        var lon = response.Lla?.Lon ?? 0;

        string mgrs = string.Empty;
        try
        {
            var coordinate = new Coordinate(lat, lon);
            mgrs = coordinate.MGRS.ToString();
        }
        catch
        {
            mgrs = "N/A";
        }

        return new LocationData
        {
            Latitude = lat,
            Longitude = lon,
            Altitude = response.Lla?.Alt ?? 0,
            Source = response.Source,
            AccuracyMeters = response.SigmaM,
            HorizontalSpeedMps = response.HorizontalSpeedMps,
            VerticalSpeedMps = response.VerticalSpeedMps,
            Mgrs = mgrs
        };
    }
}
