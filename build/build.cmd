@echo off

:: IMPORTANT!! npm 3.x is required to avoid long path exceptions

if exist "..\src\WebCompiler\node\node_modules.7z" (
echo Delete current archive.
del /Q /Y "..\src\WebCompiler\node\node_modules.7z"
)

if not exist "..\src\WebCompiler\Node" md "..\src\WebCompiler\Node"

echo Installing packages...
call npm install

if not exist "node_modules\node-sass\vendor\win32-x64-72" (
    echo Copying node binding...
    md "node_modules\node-sass\vendor\win32-x64-72"
    copy binding.node "node_modules\node-sass\vendor\win32-x64-72"
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

echo Compressing artifacts and cleans up...
7z.exe a -r -mx9 node_modules.7z node_modules > nul
copy node_modules.7z "..\src\WebCompiler\Node"
rmdir /S /Q node_modules > nul

:done
echo Done
