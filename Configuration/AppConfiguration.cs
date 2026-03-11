namespace star_map.Configuration;

/// <summary>
/// Application configuration settings
/// </summary>
public class AppConfiguration
{
    public string GrpcEndpoint { get; set; } = "192.168.100.1:9200";
    public string UartPort { get; set; } = "COM3";
    public int UartBaudRate { get; set; } = 115200;
    public bool UartEnabled { get; set; } = false;
    public bool MavlinkEnabled { get; set; } = false;
    public byte MavlinkSystemId { get; set; } = 1;
    public int UpdateIntervalMs { get; set; } = 1000;
}
