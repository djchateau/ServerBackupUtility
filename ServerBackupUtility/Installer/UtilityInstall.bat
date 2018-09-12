@ECHO OFF
CLS
ECHO.
ECHO This file installs the Server Backup Utility as a Windows Service.
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
IF NOT EXIST %SYSTEMDRIVE%\BackupUtility\ (MKDIR %SYSTEMDRIVE%\BackupUtility) > install.log
IF NOT EXIST %SYSTEMDRIVE%\BackupUtility\LogFiles (MKDIR %SYSTEMDRIVE%\BackupUtility\LogFiles) > install.log
ECHO.
ECHO Copying installation files to the Backup Utility folder.
COPY ..\Release\ServerBackupFiles.txt %SYSTEMDRIVE%\BackupUtility > install.log
COPY ..\Release\ServerBackupUtility.exe %SYSTEMDRIVE%\BackupUtility > install.log
COPY ..\Release\ServerBackupUtility.exe.config %SYSTEMDRIVE%\BackupUtility > install.log
COPY ..\Release\localhost.pfx %SYSTEMDRIVE%\BackupUtility > install.log
REM ECHO Adding the LocalService account to the Backup Utility folder's modify permissions.
REM ECHO.
REM ICACLS %SYSTEMDRIVE%\BackupUtility\ /grant:r LocalService:(OI)M > install.log
ECHO.
ECHO Please wait while the Backup Scheduler service is installed.
SC create BackupScheduler binPath= "%SYSTEMDRIVE%\BackupUtility\ServerBackupUtility.exe" start= auto > install.log
PING -n 5 127.0.0.1 > nul
ECHO.
ECHO Starting the Backup Scheduler Service.
SC start BackupScheduler > install.log
ECHO.
ECHO The Server Backup Utility installation is complete.
PAUSE
@ECHO ON
