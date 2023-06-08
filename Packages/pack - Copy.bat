@del /s *.nuspec 2>NUL
@del /s *.nupkg 2>NUL
@del /s *.snupkg 2>NUL

@if [%1]==[] (set UV_BUILD=0) else (set UV_BUILD=%1)

@for /f %%x in ('powershell -Command "[string]::Format([System.IO.File]::ReadAllText(\"../Source/VersionString.txt\"), [System.DateTime]::Now)"') do @(set UV_VERSION_MAJOR_MINOR=%%x)
@set UV_VERSION=%UV_VERSION_MAJOR_MINOR%.%UV_BUILD%

@echo Creating NuGet packages for Sedulous Framework %UV_VERSION%...

powershell -Command "(gc Sedulous.SDL2.Native.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.SDL2.Native.nuspec"
nuget pack Sedulous.SDL2.Native.nuspec
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.SDL2.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.SDL2.nuspec"
nuget pack Sedulous.SDL2.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%