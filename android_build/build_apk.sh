#!/data/data/com.termux/files/usr/bin/bash
set -e

PROJECT_DIR="/data/data/com.termux/files/home/Toot-Raft-Rush/android_build"
ANDROID_JAR="/data/data/com.termux/files/home/android-sdk/platforms/android-34/android.jar"
BUILD_DIR="$PROJECT_DIR/build"
GEN_DIR="$BUILD_DIR/gen"
OBJ_DIR="$BUILD_DIR/obj"
BIN_DIR="$BUILD_DIR/bin"
KEYSTORE="$BUILD_DIR/release.keystore"
FINAL_APK_HOME="/data/data/com.termux/files/home/TootRaftRush.apk"
FINAL_APK_REPO="/data/data/com.termux/files/home/Toot-Raft-Rush/build/TootRaftRush.apk"

echo "=========================================================="
echo "   COMPILING TOOT RAFT RUSH STANDALONE ANDROID APK       "
echo "=========================================================="

# 1. Clean and prepare build directories
rm -rf "$BUILD_DIR"
mkdir -p "$GEN_DIR" "$OBJ_DIR" "$BIN_DIR"

cd "$PROJECT_DIR"

# 2. Generate R.java with aapt
echo "[1/7] Compiling resources and generating R.java..."
aapt package -f -m \
    -J "$GEN_DIR" \
    -M AndroidManifest.xml \
    -S res \
    -I "$ANDROID_JAR"

# 3. Compile Java source code with javac
echo "[2/7] Compiling Java source files..."
javac -encoding UTF-8 \
    -source 8 -target 8 \
    -cp "$ANDROID_JAR" \
    -d "$OBJ_DIR" \
    $(find "$GEN_DIR" "$PROJECT_DIR/src" -name "*.java")

# 4. Convert .class files to DEX bytecode with d8
echo "[3/7] Converting bytecode to classes.dex with d8..."
CLASS_FILES=$(find "$OBJ_DIR" -name "*.class")
d8 --min-api 24 \
   --lib "$ANDROID_JAR" \
   --output "$BIN_DIR" \
   $CLASS_FILES

# 5. Package resources and assets into initial APK
echo "[4/7] Packaging APK resources and 3D assets..."
aapt package -f \
    -M AndroidManifest.xml \
    -S res \
    -A assets \
    -I "$ANDROID_JAR" \
    -F "$BIN_DIR/unaligned_no_dex.apk"

# Add classes.dex to the APK
cp "$BIN_DIR/classes.dex" "$BIN_DIR/classes_temp.dex"
cd "$BIN_DIR"
mv classes_temp.dex classes.dex
aapt add unaligned_no_dex.apk classes.dex
cd "$PROJECT_DIR"

# 6. Align APK with zipalign (4-byte boundary)
echo "[5/7] Aligning APK with zipalign..."
zipalign -f -p 4 "$BIN_DIR/unaligned_no_dex.apk" "$BIN_DIR/aligned.apk"

# 7. Generate keystore if not exists and sign APK
echo "[6/7] Signing APK with apksigner..."
if [ ! -f "$KEYSTORE" ]; then
    keytool -genkeypair -v \
        -keystore "$KEYSTORE" \
        -alias tootraftrush \
        -keyalg RSA \
        -keysize 2048 \
        -validity 10000 \
        -storepass tootraftrush \
        -keypass tootraftrush \
        -dname "CN=Zayn, OU=Engineering, O=TootRaftRush, C=US"
fi

mkdir -p "$(dirname "$FINAL_APK_REPO")"

apksigner sign \
    --ks "$KEYSTORE" \
    --ks-pass pass:tootraftrush \
    --key-pass pass:tootraftrush \
    --out "$FINAL_APK_HOME" \
    "$BIN_DIR/aligned.apk"

cp "$FINAL_APK_HOME" "$FINAL_APK_REPO"

# 8. Cryptographic verification
echo "[7/7] Verifying signed APK..."
apksigner verify --verbose "$FINAL_APK_HOME"

echo "=========================================================="
echo "✓ TOOT RAFT RUSH APK SUCCESSFULLY BUILT AND SIGNED!"
echo "Location 1: $FINAL_APK_HOME"
echo "Location 2: $FINAL_APK_REPO"
echo "Size: $(ls -lh "$FINAL_APK_HOME" | awk '{print $5}')"
echo "=========================================================="
