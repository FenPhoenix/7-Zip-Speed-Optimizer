echo ------------------------- START OF pre_build.bat

rem ~ strips surrounded quotes if they exist
rem batch file hell #9072: no spaces can exist around = sign for these lines
set ConfigurationName=%~1
set TargetDir=%~2
set ProjectDir=%~3
set SolutionDir=%~4
set PlatformName=%~5
set TargetFramework=%~6

echo SolutionDir: %SolutionDir%
echo PlatformName: %PlatformName%
echo TargetFramework: %TargetFramework%
