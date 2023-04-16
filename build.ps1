
Push-Location -Path "Parsing"
msbuild -t:build -restore -Property:Configuration=Release
Pop-Location

Push-Location -Path "Parsing-Test"
msbuild -t:build -restore -Property:Configuration=Release
Pop-Location

Push-Location -Path "TO2"
msbuild -t:build -restore -Property:Configuration=Release
Pop-Location

Push-Location -Path "TO2-Test"
msbuild -t:build -restore -Property:Configuration=Release
Pop-Location

Push-Location -Path "KSP2Runtime"
msbuild -t:build -restore -Property:Configuration=Release
Pop-Location

Push-Location -Path "KSP2Runtime-Test"
msbuild -t:build -restore -Property:Configuration=Release
Pop-Location

Push-Location -Path "SpaceWarpMod"
msbuild -t:build -restore -Property:Configuration=Release
Pop-Location

Push-Location -Path "Parsing-Test"
msbuild -t:test -Property:Configuration=Release
Pop-Location

Push-Location -Path "TO2-Test"
msbuild -t:test -Property:Configuration=Release
Pop-Location

Push-Location -Path "KSP2Runtime-Test"
msbuild -t:test -Property:Configuration=Release
Pop-Location

$KSP2_BASE_DIR = if($env:KSP2_BASE_DIR) { $env:KSP2_BASE_DIR } else { "C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program 2" }
if(Test-Path -Path $KSP2_BASE_DIR) {
    Copy-Item -Path "./dist/*" -Destination $KSP2_BASE_DIR -Recurse -Force
}
