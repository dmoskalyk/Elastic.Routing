set BuildProps=Configuration=Release;DelaySign=false;AssemblyOriginatorKeyFile=..\..\..\private.snk
set nuget=..\..\..\NuGet\nuget.exe
%nuget% pack ..\src\Elastic.Routing\Elastic.Routing.csproj -Build -p %BuildProps%
%nuget% pack ..\samples\Elastic.Routing.Sample\Elastic.Routing.Sample.nuspec -p %BuildProps%
pause