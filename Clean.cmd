@echo off

pushd

echo [Delete bin, obj folders]
for /f %%i in ('dir /a:d /b src\*') do call :shownomercy src\%%i

popd

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
