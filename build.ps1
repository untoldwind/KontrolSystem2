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
