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
ECHO Uninstalling the Server Backup Utility.
ECHO.
ECHO Removing installation files.
ECHO.
DEL %SYSTEMDRIVE%\BackupUtility\ServerBackupUtility.exe
DEL %SYSTEMDRIVE%\BackupUtility\ServerBackupUtility.exe.config
DEL %SYSTEMDRIVE%\BackupUtility\localhost.pfx
ECHO.
CHOICE /M "Do you want to remove the Log files?"
IF ERRORLEVEL 2 GOTO END
RMDIR /S %SYSTEMDRIVE%\BackupUtility\LogFiles
:END ECHO.
ECHO The Server Backup Utility has been uninstalled.
ECHO.
PAUSE
@ECHO ON
