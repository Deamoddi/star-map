# Starlink Location & Status Client

A .NET 10 console application for retrieving Starlink device location data via gRPC.

## 🚀 Quick Start

```bash
# 1. Check Starlink connection
ping 192.168.100.1

# 2. Run the application
dotnet run
```

See [QUICKSTART.md](QUICKSTART.md) for detailed quick start guide.

## 📖 Documentation

- **[QUICKSTART.md](QUICKSTART.md)** - Get started in 3 minutes
- **[STATUS.md](STATUS.md)** - What works and what doesn't (with real test results)
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Project architecture and design
- **[EXAMPLES.md](EXAMPLES.md)** - Code examples and use cases
- **[TROUBLESHOOTING.md](TROUBLESHOOTING.md)** - Troubleshooting guide
- **[CHANGES.md](CHANGES.md)** - Project changes and refactoring history

## Features

- ✅ **gRPC communication** with Starlink device
- ✅ **Configuration via INI file**
- ✅ **GPS coordinates** (latitude, longitude, altitude)
- ✅ **Position accuracy and speed data**
- ✅ **Position source information**
- ✅ **Object-oriented architecture** with clean separation of concerns

**Note:** Most consumer Starlink terminals do NOT support `DishGetStatus` (azimuth/elevation) via gRPC API. This functionality may only be available on enterprise/maritime terminals or through the web interface.

## Architecture

The project follows OOP principles with clear separation of responsibilities:

```
star-map/
├── Models/                      # Data models
│   ├── LocationData.cs         # GPS location data
│   ├── DishStatus.cs           # Dish status and orientation
│   └── StarlinkData.cs         # Combined data model
├── Services/                    # Business logic
│   └── StarlinkService.cs      # gRPC communication service
├── Configuration/               # Configuration management
│   ├── AppConfiguration.cs     # Configuration model
│   └── ConfigurationService.cs # INI file reader
├── Display/                     # Presentation layer
│   └── ConsoleDisplay.cs       # Console output formatting
├── Protos/                      # Protocol Buffers
│   └── starlink.proto          # gRPC service definition
├── Application.cs               # Main application coordinator
├── Program.cs                   # Entry point
└── appsettings.ini             # Configuration file
```

## Configuration

Edit `appsettings.ini` to configure the gRPC endpoint:

```ini
[Starlink]
GrpcEndpoint=192.168.100.1:9200
```

The default endpoint is `192.168.100.1:9200` if not configured.

## Usage

1. Build the project:
   ```bash
   dotnet build
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

## Output Example

```
╔════════════════════════════════════════╗
║   Starlink Location & Status Client   ║
╚════════════════════════════════════════╝

📡 Configuration
   Endpoint: 192.168.100.1:9200

⚙️  Connecting to Starlink device...
✅ Connected successfully!

╔════════════════════════════════════════╗
║         📍 LOCATION DATA               ║
╚════════════════════════════════════════╝

🌍 Coordinates:
   Latitude:        48.503210°
   Longitude:       35.954956°
   Altitude:        125.50 m

📊 Position Information:
   Source:          Starlink
   Accuracy:        5.25 m
   Horizontal Speed: 0.00 m/s
   Vertical Speed:  0.00 m/s

🔗 Connected to: 192.168.100.1:9200

Press any key to exit...
```

## Key Components

### Models
- **LocationData**: GPS coordinates, speed, and accuracy
- **StarlinkData**: Complete device data

### Services
- **StarlinkService**: Handles gRPC communication
  - `GetLocationAsync()`: Retrieves location data
  - `GetDishStatusAsync()`: ⚠️ Not supported on consumer terminals
  - `GetCompleteDataAsync()`: Retrieves location data

### Configuration
- **ConfigurationService**: Loads settings from INI file
- **AppConfiguration**: Configuration model

### Display
- **ConsoleDisplay**: Formatted console output with visual elements

### Application Flow
1. Load configuration
2. Create gRPC service
3. Retrieve location data
4. Display formatted results

## Important Limitations

⚠️ **DishGetStatus API Limitation**

Based on real-world testing, the `DishGetStatus` gRPC method (which provides azimuth, elevation, and device info) returns "Unimplemented" on consumer Starlink terminals. This appears to be a limitation of the consumer firmware.

**What works:**
- ✅ `GetLocation` - GPS coordinates, accuracy, speed

**What doesn't work on consumer terminals:**
- ❌ `DishGetStatus` - Azimuth, elevation, device info
- ❌ Device hardware/software version
- ❌ Uptime information
- ❌ Alignment statistics

**Alternatives for getting dish status:**
1. Use Starlink mobile app
2. Access web interface at http://192.168.100.1 
3. Scrape data from the web interface (not implemented in this project)
4. Use different API methods if available

The code structure still includes `DishStatus` models and methods for future compatibility or enterprise terminals that may support this functionality.

## Requirements

- .NET 10.0
- Network access to Starlink device
- Starlink device at configured IP:port

## Dependencies

- Grpc.Net.Client (2.76.0)
- Google.Protobuf (3.34.0)
- Grpc.Tools (2.78.0)

## ✅ Tested and Working

This application has been tested on a real Starlink terminal and successfully retrieves:
- GPS coordinates (Latitude: 48.503210°, Longitude: 35.954956°)
- Position accuracy
- Movement speed
- Data source information

See [STATUS.md](STATUS.md) for detailed test results and API limitations.

