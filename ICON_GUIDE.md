# 🎨 Як додати іконку

## Варіант 1: Готова іконка

1. Знайдіть або створіть іконку розміром 256x256 пікселів
2. Конвертуйте в `.ico` формат на сайті: https://icoconvert.com/
3. Збережіть файл як `icon.ico` в корені проекту
4. Перебілдіть проект

## Варіант 2: Створити іконку зі смайла 🛰️

```powershell
# PowerShell скрипт для створення простої іконки
Add-Type -AssemblyName System.Drawing

$size = 256
$bitmap = New-Object System.Drawing.Bitmap($size, $size)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)

# Фон
$brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(41, 128, 185))
$graphics.FillRectangle($brush, 0, 0, $size, $size)

# Текст
$font = New-Object System.Drawing.Font("Segoe UI Emoji", 120, [System.Drawing.FontStyle]::Bold)
$textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
$text = "🛰"
$textSize = $graphics.MeasureString($text, $font)
$x = ($size - $textSize.Width) / 2
$y = ($size - $textSize.Height) / 2
$graphics.DrawString($text, $font, $textBrush, $x, $y)

# Збереження
$bitmap.Save("icon.png", [System.Drawing.Imaging.ImageFormat]::Png)
$graphics.Dispose()
$bitmap.Dispose()

Write-Host "✅ icon.png створено!"
Write-Host "Тепер конвертуй його в .ico на https://icoconvert.com/"
```

## Варіант 3: Використати готову іконку з інтернету

Сайти з безкоштовними іконками:
- https://icons8.com/icons/set/satellite
- https://www.flaticon.com/search?word=satellite
- https://iconarchive.com/

## Що робить маніфест?

**app.manifest** налаштовує:
- ✅ Сумісність з Windows 7/8/10/11
- ✅ DPI Awareness (чіткість на 4K моніторах)
- ✅ UAC (без запиту прав адміністратора)
- ✅ Long Path Support (підтримка довгих шляхів)

## Перевірка

Після додавання іконки, перебілдіть:
```bash
.\build-release.bat
```

Іконка з'явиться у файлі `StarMap.exe`!
