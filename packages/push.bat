@echo on
for /f %%f in ('dir /b .\*.nupkg') do ..\..\..\NuGet\NuGet.exe push %%f
pause