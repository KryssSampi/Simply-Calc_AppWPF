REM set TARGET="%~dp0"..\bin\Release\net8.0-windows\Simply-Calc_AppWPF.exe""
REM for %%I in (%TARGET%) do set TARGET_FULL=%%~fI
REM if exist "%TARGET_FULL%" (start "" "%TARGET_FULL%" & exit /b 0) else (echo Fichier introuvable : "%TARGET_FULL%" & pause 1)
cd /d "%~dp0"
if not exist "..\bin\Release\net8.0-windows\Simply-Calc_AppWPF.exe" echo Fichier inexistant & pause 
start "" "..\bin\Release\net8.0-windows\Simply-Calc_AppWPF.exe"
