# Install Xamarin
Write-Host ("Installing Xamarin...")
$zipPath = "$($env:APPVEYOR_BUILD_FOLDER)\xpkg.zip"
(New-Object Net.WebClient).DownloadFile('https://components.xamarin.com/submit/xpkg', $zipPath)
7z x $zipPath | Out-Null
Set-Content -path "$env:USERPROFILE\.xamarin-credentials" -value "xamarin.com,$env:XAMARIN_COOKIE"

# Install MonoGame
Write-Host ("Installing MonoGame...")
(New-Object Net.WebClient).DownloadFile('http://www.monogame.net/releases/v3.6/MonoGameSetup.exe', 'C:\MonoGameSetup.exe')
Start-Process -FilePath "C:\MonoGameSetup.exe" -ArgumentList "/S /v /qn"

# Make sure all dependencies are fully installed
Start-Sleep -s 5