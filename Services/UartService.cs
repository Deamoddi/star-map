using System.IO.Ports;
using star_map.Models;

namespace star_map.Services;

public class UartService : IDisposable
{
    private readonly SerialPort _serialPort;
    private readonly bool _enabled;

    public UartService(string portName, int baudRate, bool enabled)
    {
        _enabled = enabled;

        if (!_enabled)
            return;

        _serialPort = new SerialPort(portName, baudRate)
        {
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
            Handshake = Handshake.None,
            ReadTimeout = 500,
            WriteTimeout = 500
        };

        try
        {
            Console.WriteLine($"🔌 Запускаємо UART: {portName} @ {baudRate} baud...");
            _serialPort.Open();
            Console.WriteLine($"✅ UART запустився!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Алярм, UART не піднявся, помилка: {ex.Message}");
            Console.WriteLine($"   Перевір чи все добре з портом");
            Console.WriteLine($"   Може порт {portName} використоовує якась інша єбала?");
            _enabled = false;
        }
    }

    public void SendLocation(LocationData location)
    {
        if (!_enabled || _serialPort == null || !_serialPort.IsOpen)
            return;

        try
        {
            // GPGGA - Essential fix data (without $ and *)
            var gpggaData = $"GPGGA,{DateTime.UtcNow:HHmmss.00}," +
                           $"{FormatCoordinate(location.Latitude, true)}," +
                           $"{FormatCoordinate(location.Longitude, false)}," +
                           $"1,08,1.0,{location.Altitude:F1},M,0.0,M,,";
            var gpgga = $"${gpggaData}*{CalculateChecksum(gpggaData)}\r\n";

            // GPRMC - Recommended minimum
            var speedKnots = location.HorizontalSpeedMps * 1.94384;
            var gprmcData = $"GPRMC,{DateTime.UtcNow:HHmmss.00},A," +
                           $"{FormatCoordinate(location.Latitude, true)}," +
                           $"{FormatCoordinate(location.Longitude, false)}," +
                           $"{speedKnots:F1},0.0,{DateTime.UtcNow:ddMMyy},,,A";
            var gprmc = $"${gprmcData}*{CalculateChecksum(gprmcData)}\r\n";

            _serialPort.Write(gpgga);
            _serialPort.Write(gprmc);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  UART помилка: {ex.Message}");
        }
    }

    private string FormatCoordinate(double coordinate, bool isLatitude)
    {
        var absCoord = Math.Abs(coordinate);
        var degrees = (int)absCoord;
        var minutes = (absCoord - degrees) * 60;

        var direction = isLatitude
            ? (coordinate >= 0 ? "N" : "S")
            : (coordinate >= 0 ? "E" : "W");

        var format = isLatitude ? "00" : "000";
        return $"{degrees.ToString(format)}{minutes:00.0000},{direction}";
    }

    private string CalculateChecksum(string data)
    {
        var checksum = 0;
        foreach (var c in data)
        {
            checksum ^= c;
        }
        return checksum.ToString("X2");
    }

    public void Dispose()
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            _serialPort.Close();
            _serialPort.Dispose();
        }
    }
}
