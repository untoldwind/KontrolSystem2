
dotnet build -restore -Property:Configuration=Release

dotnet test --logger:"console;verbosity=detailed" -Property:Configuration=Release

$KSP2_BASE_DIR = if($env:KSP2_BASE_DIR) { $env:KSP2_BASE_DIR } else { "C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program 2" }
if(Test-Path -Path $KSP2_BASE_DIR) {
    Copy-Item -Path "./dist/*" -Destination $KSP2_BASE_DIR -Recurse -Force
}
