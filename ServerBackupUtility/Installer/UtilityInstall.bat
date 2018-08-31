@ECHO OFF
CLS
ECHO.
ECHO This file installs the Server Backup Utility as a Windows application.
ECHO Run this file from an Administrator command window.
ECHO.
ECHO Open this file from the Installer folder in the temporary directory
ECHO where you unzipped the Server Backup Utility files.
ECHO.
ECHO If the current window does not have Administrator permissions,
ECHO close it now and re-open a new command window as Administrator.
ECHO.
PAUSE
ECHO.
ECHO Creating a Backup Utility folder in the system root.
ECHO.
IF NOT EXIST %SYSTEMDRIVE%\BackupUtility\ (MKDIR %SYSTEMDRIVE%\BackupUtility)
IF NOT EXIST %SYSTEMDRIVE%\BackupUtility\LogFiles (MKDIR %SYSTEMDRIVE%\BackupUtility\LogFiles)
ECHO.
ECHO Copying installation files to the Backup Utility folder.
ECHO.
COPY ..\Release\ServerBackupFiles.txt %SYSTEMDRIVE%\BackupUtility
COPY ..\Release\ServerBackupUtility.exe %SYSTEMDRIVE%\BackupUtility
COPY ..\Release\ServerBackupUtility.exe.config %SYSTEMDRIVE%\BackupUtility
COPY ..\Release\localhost.pfx %SYSTEMDRIVE%\BackupUtility
ECHO.
ECHO Adding the NetworkService account to the Backup Utility folder's modify permissions.
ECHO.
ICACLS %SYSTEMDRIVE%\BackupUtility\ /grant NetworkService:(OI)M
ECHO.
ECHO.
ECHO The Server Backup Utility installation is complete.
ECHO.
PAUSE
@ECHO ON
