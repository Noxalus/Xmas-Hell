$uri = "https://rink.hockeyapp.net/api/2/apps/$env:HOCKEYAPP_APP_ID/app_versions/upload"
$filePath = "$env:APPVEYOR_BUILD_FOLDER\$env:ANDROID_PROJECT_PATH\bin\Android\AnyCPU\Release\XmasHell-$env:APPVEYOR_BUILD_NUMBER.apk"
$method = "POST"
$param = "ipa"
$header = @{"X-HockeyAppToken"="$env:HOCKEYAPP_API_TOKEN"}
$commitSha = "$env:APPVEYOR_REPO_COMMIT"
$commitMessage = "$env:APPVEYOR_REPO_COMMIT_MESSAGE"
$repoUrl = "https://github.com/Noxalus/Xmas-Hell"
$buildServerUrl = "https://ci.appveyor.com/project/Noxalus/xmas-hell/build/$env:APPVEYOR_BUILD_VERSION"

function Get-AsciiBytes([String] $str) {
return [System.Text.Encoding]::ASCII.GetBytes($str)
}

function Send-Multipart {
  param (
    [parameter(Mandatory=$True,Position=1)] [ValidateScript({ Test-Path -PathType Leaf $_ })] [String] $FilePath,
    [parameter(Mandatory=$True,Position=2)] [System.URI] $URL,
    [parameter(Mandatory=$True,Position=3)] [String] $Method,
    [parameter(Mandatory=$True,Position=4)] [String] $Param,
    [parameter(Mandatory=$True,Position=5)] [System.Collections.IDictionary] $Headers
  )

  [byte[]]$crlf = 13, 10

  $body = New-Object System.IO.MemoryStream

  $boundary = [Guid]::NewGuid().ToString().Replace('-','')

  $encoded = Get-AsciiBytes('--' + $boundary)
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = (Get-AsciiBytes('Content-Disposition: form-data; name="status"'))
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = (Get-AsciiBytes "2")
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = Get-AsciiBytes('--' + $boundary)
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = (Get-AsciiBytes('Content-Disposition: form-data; name="notes"'))
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = (Get-AsciiBytes $commitMessage)
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = Get-AsciiBytes('--' + $boundary)
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = (Get-AsciiBytes('Content-Disposition: form-data; name="commit_sha"'))
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = (Get-AsciiBytes $commitSha)
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = Get-AsciiBytes('--' + $boundary)
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = (Get-AsciiBytes('Content-Disposition: form-data; name="repository_url"'))
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = (Get-AsciiBytes $repoUrl)
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = Get-AsciiBytes('--' + $boundary)
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = (Get-AsciiBytes('Content-Disposition: form-data; name="build_server_url"'))
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = (Get-AsciiBytes $buildServerUrl)
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = Get-AsciiBytes('--' + $boundary)
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $fileName = (Get-ChildItem $FilePath).Name
  $encoded = (Get-AsciiBytes('Content-Disposition: form-data; name="' + $Param + '"; filename="' + $fileName + '"'))
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = (Get-AsciiBytes 'Content-Type:application/octet-stream')
  $body.Write($encoded, 0, $encoded.Length)
  $body.Write($crlf, 0, $crlf.Length)
  $body.Write($crlf, 0, $crlf.Length)

  $encoded = [System.IO.File]::ReadAllBytes($filePath)
  $body.Write($encoded, 0, $encoded.Length)

  $encoded = Get-AsciiBytes('--' + $boundary)
  $body.Write($crlf, 0, $crlf.Length)
  $body.Write($encoded, 0, $encoded.Length)

  $encoded = (Get-AsciiBytes '--');
  $body.Write($encoded, 0, $encoded.Length);
  $body.Write($crlf, 0, $crlf.Length);

  try {
    Invoke-RestMethod -Headers $Headers -Uri $URL -Method $Method -ContentType "multipart/form-data; boundary=$boundary" -TimeoutSec 120 -Body $body.ToArray()
  }
  catch [System.Net.WebException] {
    Write-Error( "FAILED to reach '$URL': $_" )
    throw $_
  }
}

Send-Multipart $filePath $uri $method $param $header