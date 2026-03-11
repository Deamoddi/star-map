using System.IO.Ports;
using star_map.Models;

namespace star_map.Services;

public class MavlinkService : IDisposable
{
    private readonly SerialPort _serialPort;
    private readonly bool _enabled;
    private readonly byte _systemId;
    private readonly byte _componentId = 220;  // MAV_COMP_ID_GPS (220)
    private ushort _sequence = 0;
    private int _packetsSent = 0;

    public MavlinkService(string portName, int baudRate, bool enabled, byte systemId = 1)
    {
        _enabled = enabled;
        _systemId = systemId;

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
            Console.WriteLine($"🔌 Запускаємо MavLINK на порту {portName} з швикістю {baudRate}...");
            _serialPort.Open();
            Console.WriteLine($"✅ MAVLink стартанув!");

            // Start reading thread
            Task.Run(() => ReadLoop());

            // Send initial heartbeat to announce presence
            SendHeartbeat();
            Console.WriteLine($"💓 Відправляємо heartbeat");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Алярм, все пішло по пиз.. : {ex.Message}");
            Console.WriteLine($"  Перевір може щось не так з COM-портами");
            Console.WriteLine($"   Цей  {portName} точно не використовує якась інша єбала?");
            _enabled = false;
        }
    }

    private void ReadLoop()
    {
        byte[] buffer = new byte[1024];
        int bytesRead = 0;

        while (_enabled && _serialPort != null && _serialPort.IsOpen)
        {
            try
            {
                if (_serialPort.BytesToRead > 0)
                {
                    int count = _serialPort.Read(buffer, bytesRead, buffer.Length - bytesRead);
                    bytesRead += count;

                    // Silently consume incoming data
                    if (bytesRead > buffer.Length - 100)
                        bytesRead = 0;
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
            catch
            {
                break;
            }
        }
    }

    public void SendLocation(LocationData location)
    {
        if (!_enabled || _serialPort == null || !_serialPort.IsOpen)
            return;

        try
        {
            // Send HEARTBEAT every 1 second (every 5 packets at 200ms interval)
            if (_packetsSent % 5 == 0)
                SendHeartbeat();

            var packet = CreateGpsInputPacket(location);
            _serialPort.Write(packet, 0, packet.Length);
            _packetsSent++;

            // Silent operation - no debug output
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n⚠️  Помилка відправки MAVLink: {ex.Message}");
        }
    }

    private void SendHeartbeat()
    {
        var payload = new byte[9];
        // type (uint32) - MAV_TYPE_GCS
        payload[0] = 6; 
        payload[1] = 0;
        payload[2] = 0;
        payload[3] = 0;
        // autopilot (uint8) - MAV_AUTOPILOT_INVALID
        payload[4] = 8;
        // base_mode (uint8)
        payload[5] = 0;
        // custom_mode (uint32)
        payload[6] = 0;
        payload[7] = 0;
        payload[8] = 0;

        var packet = CreateMavlink2Packet(0, payload, 9, 50); // HEARTBEAT CRC_EXTRA = 50
        _serialPort.Write(packet, 0, packet.Length);
    }

    private byte[] CreateGpsInputPacket(LocationData location)
    {
        // MAVLink GPS_INPUT message (ID: 232)
        // MAVLink wire format: fields MUST be ordered by type size (largest first)
        var payload = new byte[65];
        var index = 0;

        // TEMPORARILY SENDING RAW ALTITUDE TO DEBUG
        // Expected: Mission Planner should show ~94m
        float altitudeToSend = (float)location.Altitude;

        // uint64 fields (8 bytes)
        var timeUsec = (ulong)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000;
        WriteUInt64(payload, ref index, timeUsec);

        // uint32 fields (4 bytes)
        WriteUInt32(payload, ref index, 0); // time_week_ms

        // int32 fields (4 bytes)
        WriteInt32(payload, ref index, (int)(location.Latitude * 1e7));  // lat
        WriteInt32(payload, ref index, (int)(location.Longitude * 1e7)); // lon

        // float fields (4 bytes each)
        WriteFloat(payload, ref index, altitudeToSend);                   // alt
        WriteFloat(payload, ref index, (float)location.AccuracyMeters / 100.0f); // hdop
        WriteFloat(payload, ref index, (float)location.AccuracyMeters / 100.0f); // vdop
        WriteFloat(payload, ref index, 0.0f);                             // vn
        WriteFloat(payload, ref index, 0.0f);                             // ve
        WriteFloat(payload, ref index, 0.0f);                             // vd
        WriteFloat(payload, ref index, 0.5f);                             // speed_accuracy
        WriteFloat(payload, ref index, (float)location.AccuracyMeters);   // horiz_accuracy
        WriteFloat(payload, ref index, (float)location.AccuracyMeters);   // vert_accuracy

        // uint16 fields (2 bytes each)
        WriteUInt16(payload, ref index, 0);    // time_week
        WriteUInt16(payload, ref index, 0);    // ignore_flags
        WriteUInt16(payload, ref index, 0);    // yaw

        // uint8 fields (1 byte each)
        payload[index++] = 0;                  // gps_id
        payload[index++] = 3;                  // fix_type (3 = 3D fix)
        payload[index++] = 8;                  // satellites_visible

        return CreateMavlink2Packet(232, payload, index, 151); // GPS_INPUT CRC_EXTRA = 151
    }

    private byte[] CreateMavlink2Packet(ushort msgId, byte[] payload, int payloadLength, byte crcExtra)
    {
        var packet = new byte[payloadLength + 12]; // MAVLink 2.0: Header(10) + payload + CRC(2)
        var index = 0;

        packet[index++] = 0xFD; // MAVLink 2.0
        packet[index++] = (byte)payloadLength;
        packet[index++] = 0; // incompat_flags
        packet[index++] = 0; // compat_flags
        packet[index++] = (byte)(_sequence++ % 256);
        packet[index++] = _systemId;
        packet[index++] = _componentId;
        packet[index++] = (byte)(msgId & 0xFF);        // msgid_low
        packet[index++] = (byte)((msgId >> 8) & 0xFF); // msgid_mid
        packet[index++] = 0;                            // msgid_high (always 0 for standard messages)

        Array.Copy(payload, 0, packet, index, payloadLength);
        index += payloadLength;

        var crc = CalculateCrc(packet, payloadLength + 10, crcExtra);
        packet[index++] = (byte)(crc & 0xFF);
        packet[index++] = (byte)(crc >> 8);

        return packet;
    }

    private ushort CalculateCrc(byte[] data, int length, byte crcExtra)
    {
        ushort crc = 0xFFFF;

        for (int i = 1; i < length; i++)
        {
            byte tmp = (byte)(data[i] ^ (crc & 0xFF));
            tmp ^= (byte)(tmp << 4);
            crc = (ushort)((crc >> 8) ^ (tmp << 8) ^ (tmp << 3) ^ (tmp >> 4));
        }

        // Add CRC_EXTRA
        byte tmp2 = (byte)(crcExtra ^ (crc & 0xFF));
        tmp2 ^= (byte)(tmp2 << 4);
        crc = (ushort)((crc >> 8) ^ (tmp2 << 8) ^ (tmp2 << 3) ^ (tmp2 >> 4));

        return crc;
    }

    private void WriteUInt64(byte[] buffer, ref int index, ulong value)
    {
        buffer[index++] = (byte)(value & 0xFF);
        buffer[index++] = (byte)((value >> 8) & 0xFF);
        buffer[index++] = (byte)((value >> 16) & 0xFF);
        buffer[index++] = (byte)((value >> 24) & 0xFF);
        buffer[index++] = (byte)((value >> 32) & 0xFF);
        buffer[index++] = (byte)((value >> 40) & 0xFF);
        buffer[index++] = (byte)((value >> 48) & 0xFF);
        buffer[index++] = (byte)((value >> 56) & 0xFF);
    }

    private void WriteUInt32(byte[] buffer, ref int index, uint value)
    {
        buffer[index++] = (byte)(value & 0xFF);
        buffer[index++] = (byte)((value >> 8) & 0xFF);
        buffer[index++] = (byte)((value >> 16) & 0xFF);
        buffer[index++] = (byte)((value >> 24) & 0xFF);
    }

    private void WriteInt32(byte[] buffer, ref int index, int value)
    {
        WriteUInt32(buffer, ref index, (uint)value);
    }

    private void WriteUInt16(byte[] buffer, ref int index, ushort value)
    {
        buffer[index++] = (byte)(value & 0xFF);
        buffer[index++] = (byte)((value >> 8) & 0xFF);
    }

    private void WriteFloat(byte[] buffer, ref int index, float value)
    {
        var bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, buffer, index, 4);
        index += 4;
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
