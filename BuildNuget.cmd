@echo off

cd "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\"
call VsDevCmd.bat

cd "%~dp0"

pushd

echo [Delete bin, obj folders]
for /f %%i in ('dir /a:d /b src\WebCompiler\*') do call :shownomercy src\%%i

popd

:build
dotnet restore src\WebCompiler\
dotnet pack -c Release .\src\WebCompiler\WebCompiler.csproj

goto :EOF

:shownomercy
if exist %1\bin (
  echo   %1\bin
  rmdir /s /q %1\bin
)
if exist %1\obj (
  echo   %1\obj
  rmdir /s /q %1\obj
)
exit /B
