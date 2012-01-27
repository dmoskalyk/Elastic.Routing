@echo on
for /f %%f in ('dir /b .\*.nupkg') do nuget.exe push %%f
pause