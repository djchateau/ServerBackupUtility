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
SC stop BackupScheduler > uninstall.log
PING -n 5 127.0.0.1 > nul
SC delete BackupScheduler > uninstall.log
ECHO.
ECHO Removing Server Backup Utility files.
DEL %SYSTEMDRIVE%\BackupUtility\ServerBackupFiles.txt > uninstall.log
DEL %SYSTEMDRIVE%\BackupUtility\ServerBackupUtility.exe > uninstall.log
DEL %SYSTEMDRIVE%\BackupUtility\ServerBackupUtility.exe.config > uninstall.log
DEL %SYSTEMDRIVE%\BackupUtility\localhost.pfx > uninstall.log
ECHO.
ECHO.
CHOICE /M "Do you want to remove the Log files?"
IF ERRORLEVEL 2 GOTO END
RMDIR /S %SYSTEMDRIVE%\BackupUtility\LogFiles > uninstall.log
:END
ECHO.
ECHO The Server Backup Utility has been removed.
PAUSE
@ECHO ON
