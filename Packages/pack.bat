@del /s *.nuspec 2>NUL
@del /s *.nupkg 2>NUL
@del /s *.snupkg 2>NUL

@if [%1]==[] (set UV_BUILD=0) else (set UV_BUILD=%1)

@for /f %%x in ('powershell -Command "[string]::Format([System.IO.File]::ReadAllText(\"../Source/VersionString.txt\"), [System.DateTime]::Now)"') do @(set UV_VERSION_MAJOR_MINOR=%%x)
@set UV_VERSION=%UV_VERSION_MAJOR_MINOR%.%UV_BUILD%

@echo Creating NuGet packages for Sedulous Framework %UV_VERSION%...

powershell -Command "(gc Sedulous.OpenGL.Environment.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.OpenGL.Environment.nuspec"
nuget pack Sedulous.OpenGL.Environment.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.OpenGL.Bindings.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.OpenGL.Bindings.nuspec"
nuget pack Sedulous.OpenGL.Bindings.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.Core.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.Core.nuspec"
nuget pack Sedulous.Core.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.Shims.Android.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.Shims.Android.nuspec"
nuget pack Sedulous.Shims.Android.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.BASS.Native.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.BASS.Native.nuspec"
nuget pack Sedulous.BASS.Native.nuspec
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.BASS.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.BASS.nuspec"
nuget pack Sedulous.BASS.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.FMOD.Native.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.FMOD.Native.nuspec"
nuget pack Sedulous.FMOD.Native.nuspec
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.FMOD.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.FMOD.nuspec"
nuget pack Sedulous.FMOD.nuspec
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.Shims.NETCore.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.Shims.NETCore.nuspec"
nuget pack Sedulous.Shims.NETCore.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.StbImageSharp.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.StbImageSharp.nuspec"
nuget pack Sedulous.StbImageSharp.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.StbImageWriteSharp.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.StbImageWriteSharp.nuspec"
nuget pack Sedulous.StbImageWriteSharp.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.nuspec"
nuget pack Sedulous.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.OpenGL.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.OpenGL.nuspec"
nuget pack Sedulous.OpenGL.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.SDL2.Native.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.SDL2.Native.nuspec"
nuget pack Sedulous.SDL2.Native.nuspec
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.SDL2.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.SDL2.nuspec"
nuget pack Sedulous.SDL2.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.Presentation.Compiler.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.Presentation.Compiler.nuspec"
nuget pack Sedulous.Presentation.Compiler.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.Presentation.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.Presentation.nuspec"
nuget pack Sedulous.Presentation.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.FreeType2.Native.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.FreeType2.Native.nuspec"
nuget pack Sedulous.FreeType2.Native.nuspec
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.FreeType2.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.FreeType2.nuspec"
nuget pack Sedulous.FreeType2.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.ImGuiViewProvider.Native.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.ImGuiViewProvider.Native.nuspec"
nuget pack Sedulous.ImGuiViewProvider.Native.nuspec
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.ImGuiViewProvider.Bindings.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.ImGuiViewProvider.Bindings.nuspec"
nuget pack Sedulous.ImGuiViewProvider.Bindings.nuspec
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.ImGuiViewProvider.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.ImGuiViewProvider.nuspec"
nuget pack Sedulous.ImGuiViewProvider.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.Tools.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.Tools.nuspec"
nuget pack Sedulous.Tools.nuspec
@if %errorlevel% neq 0 @exit /b %errorlevel%

powershell -Command "(gc Sedulous.Windows.Forms.nuspe_) -replace 'UV_VERSION', '%UV_VERSION%' | sc Sedulous.Windows.Forms.nuspec"
nuget pack Sedulous.Windows.Forms.nuspec -Symbols -SymbolPackageFormat snupkg
@if %errorlevel% neq 0 @exit /b %errorlevel%