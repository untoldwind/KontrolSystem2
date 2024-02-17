if(Test-Path -Path "bin") {
    Remove-Item "bin" -Recurse
}
if(Test-Path -Path "dist") {
    Remove-Item "dist" -Recurse
}
if(Test-Path -Path "Parsing\obj") {
    Remove-Item "Parsing\obj" -Recurse
}
if(Test-Path -Path "Parsing-Test\obj") {
    Remove-Item "Parsing-Test\obj" -Recurse
}
if(Test-Path -Path "TO2\obj") {
    Remove-Item "TO2\obj" -Recurse
}
if(Test-Path -Path "TO2-Test\obj") {
    Remove-Item "TO2-Test\obj" -Recurse
}
if(Test-Path -Path "KPS2Runtime\obj") {
    Remove-Item "KSP2Runtime\obj" -Recurse
}
if(Test-Path -Path "KPS2Runtime-Test\obj") {
    Remove-Item "KSP2Runtime-Test\obj" -Recurse
}
if(Test-Path -Path "SpaceWarap\obj") {
    Remove-Item "SpaceWarap\obj" -Recurse
}


