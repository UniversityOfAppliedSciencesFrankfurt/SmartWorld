
@ECHO ON

ECHO Changing Folder to %1
cd %1
ECHO Creating NUGET package
dnu pack
@ECHO ON

ECHO NugetPackage has completed
copy bin\debug\*.nupkg ..\..\nugetbuild