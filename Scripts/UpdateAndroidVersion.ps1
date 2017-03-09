$projectPath = "$env:APPVEYOR_BUILD_FOLDER\$env:ANDROID_PROJECT_PATH"

# Load the bootstrap file
[xml] $xam = Get-Content -Path ($projectPath + "\Properties\AndroidManifest.xml")

# Get the version from Android Manifest
$version = Select-Xml -xml $xam  -Xpath "/manifest/@android:versionCode" -namespace @{android="http://schemas.android.com/apk/res/android"}

# Increment the version
[double] $iVer = $version.Node.Value
$version.Node.Value = "$env:APPVEYOR_BUILD_NUMBER"

# Save the file
$xam.Save($projectPath + "\Properties\AndroidManifest.xml")