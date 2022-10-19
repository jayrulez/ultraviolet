@del /s *.nuspec 2>NUL
@del /s *.nupkg 2>NUL
@del /s *.snupkg 2>NUL
@if %errorlevel% neq 0 @exit /b %errorlevel%