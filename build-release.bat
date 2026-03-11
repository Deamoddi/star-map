@echo off
echo ╔════════════════════════════════════════╗
echo ║      StarMap - Release Builder         ║
echo ║           by Maestro                   ║
echo ╚════════════════════════════════════════╝
echo.

REM Налаштування
set "OUTPUT_DIR=Release"
set "RUNTIME=win-x64"

echo 🔨 Очищення старих білдів...
if exist "%OUTPUT_DIR%" rmdir /s /q "%OUTPUT_DIR%"
if exist "bin\Release" rmdir /s /q "bin\Release"
if exist "obj\Release" rmdir /s /q "obj\Release"

echo.
echo 📦 Білдимо реліз для %RUNTIME%...
dotnet publish -c Release -r %RUNTIME% ^
    --self-contained true ^
    /p:PublishSingleFile=true ^
    /p:PublishTrimmed=true ^
    /p:EnableCompressionInSingleFile=true ^
    /p:DebugType=None ^
    /p:DebugSymbols=false ^
    -o "%OUTPUT_DIR%"

if errorlevel 1 (
    echo.
    echo ❌ Білд провалився!
    pause
    exit /b 1
)

echo.
echo 🧹 Прибираємо зайве...
cd "%OUTPUT_DIR%"

REM Видаляємо .pdb файли (debug symbols)
del /q *.pdb 2>nul

REM Залишаємо тільки EXE та INI
for %%F in (*) do (
    if /i not "%%~xF"==".exe" (
        if /i not "%%~xF"==".ini" (
            if /i not "%%~xF"==".dll" (
                del /q "%%F" 2>nul
            )
        )
    )
)

REM Перевіряємо чи є appsettings.ini
if not exist "appsettings.ini" (
    echo ⚠️  appsettings.ini не знайдено, копіюємо з корня...
    copy ..\appsettings.ini . >nul
)

cd ..

echo.
echo ✅ Реліз готовий!
echo.
echo 📂 Місцезнаходження: %CD%\%OUTPUT_DIR%\
echo.

dir "%OUTPUT_DIR%\*.exe" "%OUTPUT_DIR%\*.ini" | findstr /v "Байт"

echo.
echo 📊 Розмір файлів:
for %%F in ("%OUTPUT_DIR%\StarMap.exe") do echo    StarMap.exe: %%~zF bytes (%%~zF / 1024 / 1024 MB)
for %%F in ("%OUTPUT_DIR%\appsettings.ini") do echo    appsettings.ini: %%~zF bytes

echo.
echo 🎉 Готово! Можна запускати StarMap.exe
echo.
pause
