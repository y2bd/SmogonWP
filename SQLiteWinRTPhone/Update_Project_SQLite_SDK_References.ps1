# Brian Wilhite
# bcwilhite@live.com
# TechEd 2013 NA June 5th, 2013

$FolderPath = "C:\Program Files (x86)\Microsoft SDKs\Windows Phone\v8.0\ExtensionSDKs\SQLite.WP80"
$FilePath   = ".\SQLiteWinRTPhone.vcxproj"
$VersionNumber = Get-ChildItem -Path $FolderPath | Where-Object {$_.Extension -ne ".deleteme"} | Select-Object -ExpandProperty Name
$FileContents = Get-Content -Path $FilePath
$FileContents = $FileContents | ForEach-Object {$_ -replace '<SDKReference Include="SQLite.WP80, Version=(\d.?)+"', "<SDKReference Include=`"SQLite.WP80, Version=$VersionNumber`""}
$FileContents = $FileContents | ForEach-Object {$_ -replace '<Import Project="\$\(MSBuildProgramFiles32\)\\Microsoft SDKs\\Windows Phone\\v8.0\\ExtensionSDKs\\SQLite.WP80\\(\d.?)+\\', "<Import Project=`"`$(MSBuildProgramFiles32)\Microsoft SDKs\Windows Phone\v8.0\ExtensionSDKs\SQLite.WP80\$VersionNumber\"}
$FileContents | Out-File $FilePath -Encoding ascii -Force