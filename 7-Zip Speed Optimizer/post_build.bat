echo ------------------------- START OF post_build.bat

rem ~ strips surrounded quotes if they exist
rem batch file hell #9072: no spaces can exist around = sign for these lines
set ConfigurationName=%~1
set TargetDir=%~2
set ProjectDir=%~3
set SolutionDir=%~4
set PlatformName=%~5
set TargetFramework=%~6

"%system%xcopy" "%SolutionDir%bin_dependencies\*.*" "%TargetDir%" /y /i /e

rem Dumb hack to get rid of extraneous dll files because ludicrously
rem xcopy requires you to make an entire file just to list excludes, rather than
rem specifying them on the command line like someone who is not clinically insane
del /F "%TargetDir%JetBrains.Annotations.dll"
rem del /F "%TargetDir%x86\*.*"
rem del /F "%TargetDir%x64\*.*"
echo "%TargetDir%x64"
rd /S /Q "%TargetDir%x86"
rd /S /Q "%TargetDir%x64"

rem Personal local-only file (git-ignored). It contains stuff that is only appropriate for my personal setup and
rem might well mess up someone else's. So don't worry about it.
if exist "%ProjectDir%post_build_fen_personal_dev.bat" (
	"%ProjectDir%post_build_fen_personal_dev.bat" "%ConfigurationName%" "%TargetDir%" "%ProjectDir%" "%SolutionDir%" "%PlatformName%" "%TargetFramework%"
)
