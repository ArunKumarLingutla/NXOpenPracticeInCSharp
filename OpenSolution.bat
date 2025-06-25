@echo off
setlocal

:: Search for the .sln file inside SourceCode folder
for %%f in (SourceCode\*.sln) do (
    set "SOLUTION=%%f"
    goto :found
)

echo No solution file (*.sln) found in the SourceCode folder.
goto :end

:found
echo Opening solution: %SOLUTION%

:: Launch with default associated program (usually Visual Studio)
start "" "%SOLUTION%"

:end
endlocal
