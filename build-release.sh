#!/bin/bash
echo "╔════════════════════════════════════════╗"
echo "║      StarMap - Release Builder         ║"
echo "║           by Maestro                   ║"
echo "╚════════════════════════════════════════╝"
echo ""

# Налаштування
OUTPUT_DIR="Release"
RUNTIME="linux-x64"

echo "🔨 Очищення старих білдів..."
rm -rf "$OUTPUT_DIR"
rm -rf "bin/Release"
rm -rf "obj/Release"

echo ""
echo "📦 Білдимо реліз для $RUNTIME..."
dotnet publish -c Release -r $RUNTIME \
    --self-contained true \
    /p:PublishSingleFile=true \
    /p:PublishTrimmed=true \
    /p:EnableCompressionInSingleFile=true \
    /p:DebugType=None \
    /p:DebugSymbols=false \
    -o "$OUTPUT_DIR"

if [ $? -ne 0 ]; then
    echo ""
    echo "❌ Білд провалився!"
    exit 1
fi

echo ""
echo "🧹 Прибираємо зайве..."
cd "$OUTPUT_DIR"

# Видаляємо .pdb файли (debug symbols)
rm -f *.pdb

# Залишаємо тільки StarMap та INI
find . -type f ! -name 'StarMap' ! -name 'appsettings.ini' ! -name '*.so' -delete

# Перевіряємо чи є appsettings.ini
if [ ! -f "appsettings.ini" ]; then
    echo "⚠️  appsettings.ini не знайдено, копіюємо з корня..."
    cp ../appsettings.ini .
fi

# Робимо виконуваним
chmod +x StarMap

cd ..

echo ""
echo "✅ Реліз готовий!"
echo ""
echo "📂 Місцезнаходження: $(pwd)/$OUTPUT_DIR/"
echo ""

ls -lh "$OUTPUT_DIR/StarMap" "$OUTPUT_DIR/appsettings.ini"

echo ""
echo "🎉 Готово! Можна запускати ./StarMap"
echo ""
