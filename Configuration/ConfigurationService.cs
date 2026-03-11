namespace star_map.Configuration;

public class ConfigurationService
{
    private const string DefaultConfigPath = "appsettings.ini";

    public AppConfiguration LoadConfiguration(string configPath = DefaultConfigPath)
    {
        var config = new AppConfiguration();

        if (!File.Exists(configPath))
        {
            Console.WriteLine($"⚠️  Хтось проїбав конфіг, застосовуються дефолти");
            return config;
        }

        try
        {
            var lines = File.ReadAllLines(configPath);
            string currentSection = "";

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    currentSection = trimmed.Trim('[', ']');
                    continue;
                }

                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#") || trimmed.StartsWith(";"))
                    continue;

                var parts = trimmed.Split('=', 2);
                if (parts.Length != 2) continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                if (currentSection == "Starlink" && key == "GrpcEndpoint")
                    config.GrpcEndpoint = value;
                else if (currentSection == "UART")
                {
                    if (key == "Port") config.UartPort = value;
                    else if (key == "BaudRate") config.UartBaudRate = int.Parse(value);
                    else if (key == "Enabled") config.UartEnabled = bool.Parse(value);
                }
                else if (currentSection == "MAVLink")
                {
                    if (key == "Enabled") config.MavlinkEnabled = bool.Parse(value);
                    else if (key == "SystemId") config.MavlinkSystemId = byte.Parse(value);
                }
                else if (currentSection == "Polling" && key == "UpdateIntervalMs")
                    config.UpdateIntervalMs = int.Parse(value);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Error reading config: {ex.Message}");
        }

        return config;
    }
}
