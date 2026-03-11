# 🛰️ StarMap - Starlink GPS to MAVLink Bridge

**by Maestro**

## 📝 Опис

StarMap - це утиліта для отримання GPS координат зі Starlink та відправки їх в ArduPilot через MAVLink.

## 📦 Склад релізу

- `StarMap.exe` - головний виконуваний файл (всі залежності вже включені)
- `appsettings.ini` - конфігураційний файл

## 🚀 Швидкий старт

1. Переконайтеся що Starlink підключений до мережі
2. Відредагуйте `appsettings.ini` якщо потрібно
3. Запустіть `StarMap.exe`

## ⚙️ Налаштування (appsettings.ini)

```ini
[Starlink]
GrpcEndpoint=192.168.100.1:9200    # IP адреса Starlink

[UART]
Enabled=false                       # Увімкнути NMEA через UART
Port=COM29                          # COM порт
BaudRate=115200                     # Швидкість

[MAVLink]
Enabled=true                        # Увімкнути MAVLink
SystemId=1                          # System ID (має збігатися з автопілотом)

[Polling]
UpdateIntervalMs=200                # Інтервал оновлення в мілісекундах
```

## 🔧 Вимоги

- Windows 10/11 (x64)
- .NET Runtime 10.0 (вже включено в EXE)
- Starlink підключений до мережі
- ArduPilot з налаштованим MAVLink портом (опціонально)

## 📡 Налаштування ArduPilot

В Mission Planner встановіть параметри:

```
GPS_TYPE = 14           (MAVLink)
SERIAL3_PROTOCOL = 2    (MAVLink 2)
SERIAL3_BAUD = 115      (115200)
```

## 💡 Поради

1. Перевірте підключення до Starlink: `ping 192.168.100.1`
2. В Device Manager перевірте номер COM порту
3. System ID має збігатися з автопілотом (зазвичай 1)
4. Для точнішої висоти можна додати geoid correction в коді

## 🐛 Проблеми

**"Не вдалось законектитись зі старом"**
- Перевірте чи Starlink увімкнутий
- Перевірте IP адресу в `appsettings.ini`
- Спробуйте `ping 192.168.100.1`

**"Failed to open MAVLink"**
- Перевірте номер COM порту в Device Manager
- Закрийте інші програми що використовують цей порт (Mission Planner, тощо)
- Перевірте що автопілот підключений

## 📊 Технічні деталі

- GPS_INPUT message (ID: 232)
- MAVLink 2.0 protocol
- Component ID: 220 (MAV_COMP_ID_GPS)
- Підтримка WGS84 висоти
- Інтервал оновлення: 200мс (5 Hz)

## 📄 Ліцензія

© 2025 Maestro. All rights reserved.

---

**Версія:** 1.0.0  
**Дата:** 2025-01-14
