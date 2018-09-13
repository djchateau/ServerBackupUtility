@ECHO OFF
CLS
ECHO.
ECHO This file uninstalls the Server Backup Utility.
ECHO Run this file from an Administrator command window.
ECHO.
ECHO If the current window does not have Administrator permissions,
ECHO close it now and re-open a new command window as Administrator.
ECHO.
PAUSE
ECHO.
ECHO Please wait while the Backup Scheduler service is uninstalled.
SC stop BackupScheduler > nul
PING -n 5 127.0.0.1 > nul
SC delete BackupScheduler > nul
ECHO.
ECHO Removing application files.
DEL %SYSTEMDRIVE%\BackupUtility\RestartScheduler.bat > nul
DEL %SYSTEMDRIVE%\BackupUtility\ServerBackupFiles.txt > nul
DEL %SYSTEMDRIVE%\BackupUtility\ServerBackupUtility.exe > nul
DEL %SYSTEMDRIVE%\BackupUtility\ServerBackupUtility.exe.config > nul
DEL %SYSTEMDRIVE%\BackupUtility\localhost.pfx > nul
ECHO.
CHOICE /M "Do you want to remove the Server Backup Utility Log files?"
IF ERRORLEVEL 2 GOTO END
RMDIR /S %SYSTEMDRIVE%\BackupUtility\LogFiles > nul
:END
ECHO.
ECHO The Server Backup Utility was successfully removed.
ECHO.
PAUSE
@ECHO ON
