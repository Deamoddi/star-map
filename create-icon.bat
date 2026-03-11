@echo off
echo Creating icon.png...

powershell -Command "& { Add-Type -AssemblyName System.Drawing; $size = 256; $bitmap = New-Object System.Drawing.Bitmap($size, $size); $graphics = [System.Drawing.Graphics]::FromImage($bitmap); $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias; $graphics.Clear([System.Drawing.Color]::FromArgb(41, 128, 185)); $font = New-Object System.Drawing.Font('Segoe UI', 180, [System.Drawing.FontStyle]::Bold); $textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White); $text = 'S'; $format = New-Object System.Drawing.StringFormat; $format.Alignment = [System.Drawing.StringAlignment]::Center; $format.LineAlignment = [System.Drawing.StringAlignment]::Center; $rect = New-Object System.Drawing.RectangleF(0, 0, $size, $size); $graphics.DrawString($text, $font, $textBrush, $rect, $format); $bitmap.Save('icon.png', [System.Drawing.Imaging.ImageFormat]::Png); $graphics.Dispose(); $bitmap.Dispose(); Write-Host 'icon.png created!' }"

echo.
echo ✅ icon.png створено!
echo.
echo 📝 Тепер:
echo    1. Відкрий icon.png
echo    2. Якщо треба - відредагуй в Paint/Photoshop
echo    3. Конвертуй в .ico на https://icoconvert.com/
echo    4. Збережи як icon.ico в корені проекту
echo    5. Запусти build-release.bat
echo.
pause
