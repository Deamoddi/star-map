# 🛰️ StarMap - Starlink GPS → MAVLink Bridge

**by Maestro**

Утиліта для отримання GPS координат зі Starlink та відправки їх в ArduPilot через MAVLink.

---

## 🚀 Швидкий старт

```bash
# 1. Перевір підключення до Starlink
ping 192.168.100.1

# 2. Запусти програму
dotnet run

# АБО білд релізу
.\build-release.bat
cd Release
.\StarMap.exe
```

---

## ✨ Можливості

- ✅ **gRPC зв'язок** зі Starlink
- ✅ **GPS координати** (широта, довгота, висота)
- ✅ **MAVLink протокол** для ArduPilot
- ✅ **NMEA вивід** через UART (опціонально)
- ✅ **Конфігурація через INI** файл
- ✅ **Geoid correction** для точної висоти
- ✅ **Single-file EXE** без залежностей
- ✅ **Українська локалізація** з підтримкою емодзі

---

## 📁 Структура проекту

```
star-map/
├── 📁 Configuration/            # Конфігурація
│   ├── AppConfiguration.cs     # Модель налаштувань
│   └── ConfigurationService.cs # Читання INI
├── 📁 Display/                  # Відображення
│   └── ConsoleDisplay.cs       # Консольний вивід
├── 📁 Models/                   # Моделі даних
│   ├── LocationData.cs         # GPS дані
│   ├── DishStatus.cs           # Статус тарілки
│   └── StarlinkData.cs         # Загальні дані
├── 📁 Services/                 # Сервіси
│   ├── StarlinkService.cs      # gRPC зв'язок
│   ├── MavlinkService.cs       # MAVLink протокол
│   └── UartService.cs          # NMEA вивід
├── 📁 Protos/                   # Protocol Buffers
│   └── starlink.proto          # gRPC схема
├── 📁 Release/                  # ⭐ Готовий EXE
│   ├── StarMap.exe             # Виконуваний файл (15.26 MB)
│   └── appsettings.ini         # Конфіг
├── 📄 Application.cs            # Головна логіка
├── 📄 Program.cs                # Entry point
├── 📄 appsettings.ini           # Конфігурація
├── 📄 app.manifest              # Windows маніфест
├── 📄 icon.ico                  # Іконка програми
├── 📄 star-map.csproj           # Проект файл
├── 🔨 build-release.bat         # Білд для Windows
├── 🔨 build-release.sh          # Білд для Linux
└── 📖 RELEASE.md                # Документація користувача
```

---

## ⚙️ Конфігурація (appsettings.ini)

```ini
[Starlink]
GrpcEndpoint=192.168.100.1:9200    # IP адреса Starlink

[UART]
Enabled=false                       # Увімкнути NMEA через UART
Port=COM29                          # COM порт
BaudRate=115200                     # Швидкість передачі

[MAVLink]
Enabled=true                        # Увімкнути MAVLink
SystemId=1                          # System ID (має збігатися з автопілотом)

[Polling]
UpdateIntervalMs=200                # Інтервал оновлення (мс)
```

---

## 🔧 Налаштування ArduPilot

В **Mission Planner** встановіть параметри:

```
GPS_TYPE = 14           (MAVLink)
SERIAL3_PROTOCOL = 2    (MAVLink 2)
SERIAL3_BAUD = 115      (115200 baud)
```

Опціонально (для використання як основного джерела позиції):
```
EK3_SRC1_POSXY = 6      (ExternalNav)
EK3_SRC1_POSZ = 6       (ExternalNav)
```

---

## 💻 Розробка

### Вимоги
- .NET 10 SDK
- Visual Studio 2026 або VS Code
- Starlink підключений до мережі

### Білд проекту
```bash
# Звичайний білд
dotnet build

# Release білд (single-file EXE)
.\build-release.bat

# Для Linux
chmod +x build-release.sh
./build-release.sh
```

### Запуск з коду
```bash
dotnet run
```

---

## 📡 Технічні деталі

### MAVLink
- **Protocol:** MAVLink 2.0
- **Message ID:** 232 (GPS_INPUT)
- **Component ID:** 220 (MAV_COMP_ID_GPS)
- **System ID:** Конфігурується (за замовчуванням 1)
- **Частота оновлення:** 5 Hz (200ms)

### GPS дані
- **Формат:** WGS84 (Starlink) → AMSL (ArduPilot)
- **Geoid correction:** -27m для України
- **Точність:** ~0.37m горизонтальна
- **Підтримка:** Lat, Lon, Alt, HDOP, VDOP, швидкість

### gRPC
- **Endpoint:** 192.168.100.1:9200
- **Protocol:** HTTP/2
- **Messages:** GetLocation, DishGetStatus (обмежено)

---

## 🐛 Вирішення проблем

### "Не вдалось законектитись зі старом"
1. Перевірте чи Starlink увімкнутий
2. Перевірте IP адресу: `ping 192.168.100.1`
3. Перевірте `appsettings.ini`

### "Failed to open MAVLink"
1. Перевірте номер COM порту в Device Manager
2. Закрийте Mission Planner перед запуском
3. Переконайтеся що автопілот підключений

### Неправильна висота в Mission Planner
- Висота корегується geoid offset (-27m для України)
- Перевірте `AMSL` vs `WGS84` в ArduPilot
- Можна змінити offset в коді `MavlinkService.cs`

---

## 📊 Вивід програми

```
╔════════════════════════════════════════╗
║           StarMap by Maestro           ║
╚════════════════════════════════════════╝

📡 Конфігурація
   Starlink gRPC IP: 192.168.100.1:9200

📋 Конфігурація:
   UART увімкнуто: False
   MAVLink увімкнуто: True
   MAVLink System ID: 1
   UART порт: COM29
   UART швидкість: 115200
   Час оновлення: 200мс

⚙️  Підлючення до тарілки...
✅ Конект пройшов!

╔════════════════════════════════════════╗
║       📍 GPS дані з тарілки            ║
╚════════════════════════════════════════╝

🌍 Координати:
   Широта:        48.502853°
   Довгота:       35.956163°
   Висота:        94.1 m
   MGRS:          36U YU 18353 76413

📊 Інформація про позиціонування:
   Джерело:       Starlink
   Точність:      0.37 м
   Горизонтальна швидкість: 0.00 м/с
   Вертикальна швидкість:   0.00 м/с

🔗 Підключено до: 192.168.100.1:9200

✅ 🔄 Починаємо відправку кожних 200мс (MAVLink)

Натисни будь-яку клавішу для зупинки...

[17:23:06] #0009 | Широта: 48.502835 Довгота: 35.956151 Висота: 94.9м | MGRS: 36U YU 18352 76411
```

---

## 🏗️ Архітектура

### Патерни
- **OOP** - об'єктно-орієнтований підхід
- **Service pattern** - розділення логіки на сервіси
- **Configuration** - INI-based конфігурація
- **Display** - окремий шар для виводу

### Потік даних
```
Starlink (gRPC) → StarlinkService → LocationData → MavlinkService → ArduPilot
                                                  ↓
                                            ConsoleDisplay
```

---

## 📦 Release білд

**Що включено:**
- Single-file EXE (всі залежності включені)
- Self-contained (.NET Runtime не потрібен)
- Trimmed (оптимізовано розмір)
- Compressed (стиснення)
- No debug symbols (без .pdb файлів)

**Розмір:** ~15.26 MB

---

## 📄 Ліцензія

© 2025 Maestro. All rights reserved.

---

## 🔗 Корисні посилання

- [MAVLink Protocol](https://mavlink.io/)
- [ArduPilot Documentation](https://ardupilot.org/)
- [Starlink gRPC API](https://github.com/sparky8512/starlink-grpc-tools)

---

**Версія:** 1.0.0  
**Дата:** 2025-01-14  
**Автор:** Maestro  
**GitHub:** https://github.com/Deamoddi/star-map
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

