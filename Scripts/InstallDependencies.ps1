# Install Xamarin
Write-Host "Installing Xamarin..."
$zipPath = "$($env:APPVEYOR_BUILD_FOLDER)\xpkg.zip"
(New-Object Net.WebClient).DownloadFile('https://components.xamarin.com/submit/xpkg', $zipPath)
7z x $zipPath | Out-Null
Set-Content -path "$env:USERPROFILE\.xamarin-credentials" -value "xamarin.com,$env:XAMARIN_COOKIE"


# Install MonoGame
Write-Host "Installing MonoGame..."
(New-Object Net.WebClient).DownloadFile('http://www.monogame.net/releases/v3.6/MonoGameSetup.exe', 'C:\MonoGameSetup.exe')
Invoke-Command -ScriptBlock {C:\MonoGameSetup.exe /S /v/qn}

# Make sure all dependencies are fully installed
Start-Sleep -s 10

Get-ChildItem "C:\Program Files (x86)\MSBuild\"
Get-ChildItem "C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools"