@echo off
cd /d "%~dp0"
echo Diretorio atual: %CD%
echo.
echo Iniciando build do APK Android...
echo.
eas build -p android --profile preview
pause
