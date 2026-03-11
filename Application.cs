using star_map.Configuration;
using star_map.Display;
using star_map.Services;

namespace star_map;

/// <summary>
/// Main application class coordinating all services
/// </summary>
public class Application
{
    private readonly ConfigurationService _configService;
    private readonly ConsoleDisplay _display;

    public Application()
    {
        _configService = new ConfigurationService();
        _display = new ConsoleDisplay();
    }

    /// <summary>
    /// Runs the application
    /// </summary>
    public async Task RunAsync()
    {
        _display.DisplayHeader();

        try
        {
            var config = _configService.LoadConfiguration();
            _display.DisplayConfiguration(config.GrpcEndpoint);

            Console.WriteLine($"\n📋 Конфігурація:");
            Console.WriteLine($"   UART увімкнуто: {config.UartEnabled}");
            Console.WriteLine($"   MAVLink увімкнуто: {config.MavlinkEnabled}");
            Console.WriteLine($"   MAVLink System ID: {config.MavlinkSystemId}");
            Console.WriteLine($"   UART порт: {config.UartPort}");
            Console.WriteLine($"   UART швидкість: {config.UartBaudRate}");
            Console.WriteLine($"   Час оновлення: {config.UpdateIntervalMs}мс\n");

            using var starlinkService = new StarlinkService(config.GrpcEndpoint);
            using var uartService = config.UartEnabled 
                ? new UartService(config.UartPort, config.UartBaudRate, config.UartEnabled) 
                : null;
            using var mavlinkService = config.MavlinkEnabled 
                ? new MavlinkService(config.UartPort, config.UartBaudRate, config.MavlinkEnabled, config.MavlinkSystemId) 
                : null;

            _display.DisplayStatus("Підлючення до тарілки...");

            var data = await starlinkService.GetCompleteDataAsync();
            _display.DisplaySuccess("Конект пройшов!");
            _display.DisplayCompleteData(data);

            if (config.UartEnabled || config.MavlinkEnabled)
            {
                Console.WriteLine();
                var protocol = config.MavlinkEnabled ? "MAVLink" : "NMEA/UART";
                _display.DisplaySuccess($"🔄 Починаємо відправку кожних {config.UpdateIntervalMs}мс ({protocol})");
                Console.WriteLine("Натисни будь-яку клавішу для зупинки...\n");

                await RunPeriodicUpdates(starlinkService, uartService, mavlinkService, config);
            }
            else
            {
                _display.DisplayWaitPrompt();
                Console.ReadKey();
            }
        }
        catch (Exception ex)
        {
            _display.DisplayError("Не вдалось законектитись зі старом", ex);
            Console.WriteLine();
            Console.WriteLine("💡 Спробуй:");
            Console.WriteLine("   1. Стар точно увімкнутий?");
            Console.WriteLine("   2. Мережа піднялась?");
            Console.WriteLine("   3. Перевір IP-шку в appsettings.ini");
            Console.WriteLine("   4. Спробуй пінганути: ping 192.168.100.1");
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Runs periodic updates in a loop
    /// </summary>
    private async Task RunPeriodicUpdates(StarlinkService starlinkService, UartService? uartService, MavlinkService? mavlinkService, AppConfiguration config, int startCount = 0)
    {
        int count = startCount;
        while (!Console.KeyAvailable)
        {
            try
            {
                count++;
                var data = await starlinkService.GetCompleteDataAsync();

                if (config.UartEnabled && uartService != null)
                    uartService.SendLocation(data.Location);

                if (config.MavlinkEnabled && mavlinkService != null)
                    mavlinkService.SendLocation(data.Location);

                var status = $"[{DateTime.Now:HH:mm:ss}] #{count:D4} | Широта: {data.Location.Latitude:F6} Довгота: {data.Location.Longitude:F6} Висота: {data.Location.Altitude:F1}м | MGRS: {data.Location.Mgrs}";
                Console.Write($"\r{status}");
            }
            catch (Exception ex)
            {
                Console.Write($"\r[{DateTime.Now:HH:mm:ss}] ❌ Помилка: {ex.Message.PadRight(80)}");
            }

            await Task.Delay(config.UpdateIntervalMs);
        }

        // Clear the key that was pressed
        Console.ReadKey(true);

        // Ask for confirmation
        Console.WriteLine("\n");
        Console.Write("⚠️  Точно хочеш зупинити? (Y/N): ");
        var confirmation = Console.ReadKey();
        Console.WriteLine();

        if (confirmation.Key == ConsoleKey.Y)
        {
            Console.WriteLine("\n⏹️  Зупинено");
        }
        else
        {
            Console.WriteLine("\n🔄 Продовжуємо...\n");
            await RunPeriodicUpdates(starlinkService, uartService, mavlinkService, config, count);
        }
    }
}
