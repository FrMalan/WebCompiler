@echo off

:: IMPORTANT!! npm 3.x is required to avoid long path exceptions

echo Build folder for node_module preparation:
echo %~dp0

if exist "%~dp0..\src\WebCompiler\node\node_modules.7z" (
echo src\WebCompiler\node\node_modules.7z already exists. Manually remove to regenerate.
exit
)

if not exist "%~dp0..\src\WebCompiler\Node" md "..\src\WebCompiler\Node"

pushd

cd %~dp0

REM Only update node-sass in package.json if you are running update NodeJS version. See https://www.npmjs.com/package/node-sass
echo Installing npm packages...
call npm install

if not exist "%~dp0node_modules\node-sass\vendor\win32-x64-72" (
    echo Copying node binding...
    md "%~dp0node_modules\node-sass\vendor\win32-x64-72"
    copy binding.node "%~dp0node_modules\node-sass\vendor\win32-x64-72"
)

echo Deleting unneeded files and folders...
cd node_modules
del /s /q *.html > nul
del /s /q *.markdown > nul
del /s /q *.md > nul
del /s /q *.npmignore > nul
del /s /q *.patch > nul
del /s /q *.txt > nul
del /s /q *.yml > nul
del /s /q .editorconfig > nul
del /s /q .eslintrc > nul
del /s /q .gitattributes > nul
del /s /q .jscsrc > nul
del /s /q .jshintrc > nul
del /s /q CHANGELOG > nul
del /s /q CNAME > nul
del /s /q example.js > nul
del /s /q generate-* > nul
del /s /q gruntfile.js > nul
del /s /q gulpfile.* > nul
del /s /q makefile.* > nul
del /s /q README > nul

echo Deleting more stuff...
for /d /r . %%d in (benchmark)  do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (bench)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (doc)        do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (docs)       do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (example)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (examples)   do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (images)     do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (man)        do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (media)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (scripts)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (test)       do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (tests)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (testing)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (tst)        do @if exist "%%d" rd /s /q "%%d" > nul

cd ..

echo Compressing node_modules...
7z.exe a -r -mx9 node_modules.7z node_modules > nul
echo Copy files...
copy %~dp0node_modules.7z "..\src\WebCompiler\Node"
copy %~dp0node.7z "..\src\WebCompiler\Node"
echo Clean up...
rmdir /S /Q node_modules > nul

:done
popd
echo Done
