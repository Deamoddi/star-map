using SpaceX.API.Device;

namespace star_map.Models;

/// <summary>
/// Represents dish status including alignment and orientation data
/// </summary>
public class DishStatus
{
    public string DeviceId { get; set; } = string.Empty;
    public string HardwareVersion { get; set; } = string.Empty;
    public string SoftwareVersion { get; set; } = string.Empty;
    public ulong UptimeSeconds { get; set; }
    
    // Orientation data
    public double BoresightAzimuthDeg { get; set; }
    public double BoresightElevationDeg { get; set; }
    public double DesiredBoresightAzimuthDeg { get; set; }
    public double DesiredBoresightElevationDeg { get; set; }

    public static DishStatus FromResponse(DishGetStatusResponse response)
    {
        var alignmentStats = response.AlignmentStats;
        
        return new DishStatus
        {
            DeviceId = response.DeviceInfo?.Id ?? "Unknown",
            HardwareVersion = response.DeviceInfo?.HardwareVersion ?? "Unknown",
            SoftwareVersion = response.DeviceInfo?.SoftwareVersion ?? "Unknown",
            UptimeSeconds = response.DeviceState?.UptimeS ?? 0,
            BoresightAzimuthDeg = alignmentStats?.BoresightAzimuthDeg ?? response.BoresightAzimuthDeg,
            BoresightElevationDeg = alignmentStats?.BoresightElevationDeg ?? response.BoresightElevationDeg,
            DesiredBoresightAzimuthDeg = alignmentStats?.DesiredBoresightAzimuthDeg ?? 0,
            DesiredBoresightElevationDeg = alignmentStats?.DesiredBoresightElevationDeg ?? 0
        };
    }
}
