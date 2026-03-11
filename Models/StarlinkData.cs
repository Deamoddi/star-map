namespace star_map.Models;

/// <summary>
/// Complete Starlink device data including location and status
/// </summary>
public class StarlinkData
{
    public LocationData? Location { get; set; }
    public DishStatus? Status { get; set; }
    public string Endpoint { get; set; } = string.Empty;
}
