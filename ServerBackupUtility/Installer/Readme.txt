
ServerBackupUtility
-------------------

Utility Application to Backup Files from a Windows Server to an FTP Server

Dependencies: This Windows application requires the .Net Framework v.4.7.2 to be installed on the server.

You can download the v.4.7.2 run-time here:
    https://www.microsoft.com/net/download/thank-you/net472

You can download the v.4.7.2 developer pack here:
    https://go.microsoft.com/fwlink/?LinkId=863261

A gracious thank you goes out to Caphyon Ltd. for providing us with a copy of their Advanced Installer program.
If you need an installer application, check it out. It's the best one I've found.
    https://www.advancedinstaller.com


Server Backup Utility Installation Instructions
-----------------------------------------------
If all else fails, read the directions...

1) Unzip the ServerBackupUtility.zip file into a temporary folder.
2) Add the relevent entries to the DirectBackupPaths.txt file in the Release folder (see below).
3) Add the relevent entries to the ServerBackupUtility.exe.config file in the Release folder (see below).
4) Switch to the Installation folder and right click on the UtilityInstall.bat file. Select Run as Administrator.
5) The batch file will create a system drive \BackupUtility folder and install itself as a Windows Service.
6) The batch file will set permissions for Local System on the Backup Scheduler service that runs the utility.

To upgrade an existing version of the utility, run the UtilityUninstall.bat file,
then run the UtilityInstall.bat file. Always run these files in Administrator mode.

Questions or Problems: You can reach me at fwchateau@gmail.com. Add Server Backup Utility to the subject line.


DirectBackupPaths.txt File
--------------------------

Each line should contain a folder path ending with a file name, or a pattern match to a set of file names.
The file match pattern is recursive below the target folder. This can be changed in the UploadService
code using SearchOptions .TopDirectoriesOnly.

Pattern Match
---------------------
\* = wildcard match (matches anything)
? = character match (matches one character)

- Back-up all files in the \console folder \
	C:\csvn\data\dumps\console\\*

- Back-up the "identity.exe" file in the \identity folder \
	C:\csvn\data\dumps\identity\identity.exe

- Back-up all files in the \maps folder with the file extension ".zip" \
	C:\csvn\data\dumps\maps\\*.zip

- Back-up all files in the \samples folder with the file name "logs" and any extension \
	C:\csvn\data\dumps\samples\logs.*

- Back-up all files in the \database folder with the file name "log[any character][any character].zip" \
	C:\csvn\data\dumps\database\log??.zip


ServerBackupUtility.exe.config File
-----------------------------------

- Scheduler Mode - Select Clock for daily backup on a regular schedule or Interval for repetitive tasks. \
    add key="Mode" value="clock"

- Clock Time in 24 Hour Format \
    add key="Clock" value="03:00"

- Interval Time in Minutes \
    add key="Interval" value="60"

- List of folders separated by pipe characters, containing target sub-folders to be archived for backup.
    You cannot archive a root folder. Place your files in a sub-folder and add the root folder here. \
	add key="ArchivePaths" value="D:\Webs|D:\Media"

- Folder to place archived files for backup. All files in this folder will be transfered to the FTP server.
    You can place files manually in this folder. The application will place the archived files here. \
	add key="BackupPath" value="D:\Backups"
   
- Folder to place the database archives. We do not recommend archiving database files with this utility.
    Use your database maintenance utility to archive your databases and place the backups in this folder. \
	add key="DatabasePath" value="D:\SqlServerDataFiles\Backup"

- You can use this setting so old backup files don't accumulate on the Windows server. Just delete
    the archived files from the Windows Server after they have been transferred to the FTP server. \
    add key="DeleteFiles" value="true"

- Url of the FTP server \
	add key="FtpUrl" value="online-server.com"

- User Name for the FTP server account \
	add key="FtpUserName" value="backupUser"

- Password for the FTP Server user \
	add key="FtpPassword" value="password"

- Select Active or Passive FTP file transfer. \
	add key="FtpMode" value="active"

- Usually port 21 here for Active transfer and port 990 for Passive transfer, but not necessarily.
    Check the configuration on your FTP server. \
	add key="FtpPort" value="21"

- Select SSL/TLS Encrypted or Unencrypted file transfer. SSH is not supported. \
	add key="FtpSsl" value="false"

- Use the Email SMTP client to send log notifications. \
	add key="EmailService" value="true"

The following settings are only required if the Email Service setting is enabled.

- SMTP user account name \
	add key="SmptUserName" value="services"

- SMTP user account password \
	add key="SmptPassword" value="password"

- Domain Name of the SMTP server \
	add key="SmptHost" value="mail-server.com"

- SMTP Server access port - Usually port 25 for unencrypted transfer \
	add key="SmptPort" value="25"

- Select SSL/TLS Encrypted or Unencrypted file transfer \
	add key="SmtpSsl" value="false"

- Email sender address \
	add key="SmtpSender" value="services@mail-server.com"

- Email recipient address \
	add key="SmtpRecipient" value="admin@mail-server.com"


Permissions
---------------------

Permissions can be a little confusing because of all the folders involved in backing up the files.
The easiest way to get things up and running is to install the utility using the Local System
account. All folders and files you will be accessing include Local System permissions by default,
but in the unlikely event you get hacked, I don't want to be responsible for it, so I'm adding
this warning. You may want to change the permissions to Local Service or Network Service, which
can have stronger security effect on the server. To do this you must add the Local Service or
Network Service accounts to the Backup Scheduler service and to any folders and files the service
touches, or you will get access errors.

You can install the Backup Scheduler service using the Local Service or Network Service accounts
by adding the following text to the UtilityInstall batch file. Leave the password field empty.

Local System: SC create BackupScheduler \
binPath= "%SYSTEMDRIVE%\BackupUtility\ServerBackupUtility.exe" start= auto > install.log

Local Service: SC create BackupScheduler \
binPath= "%SYSTEMDRIVE%\BackupUtility\ServerBackupUtility.exe" start= auto obj= "NT AUTHORITY\LOCAL SERVICE" \
password= "" > install.log

Network Service: SC create BackupScheduler \
binPath= "%SYSTEMDRIVE%\BackupUtility\ServerBackupUtility.exe" start= auto obj= "NT AUTHORITY\NETWORK SERVICE" \
password= "" > install.log

We are currently in the process of adding an installer program to the application. Until it is implemented,
if you're having trouble installing the permissions, write to me and I'll walk you through them.


Miscellaneous Notes
---------------------

If you change a setting in the Config file while the utility is running, the Scheduler will automatically restart
itself to pick up the new setting. You can also restart the Scheduler by running the RestartScheduler.bat file.

Because of the way we use dates as back-up folders on the FTP server, it is not a good idea to set the start time
too close to Midnight. For example, if you have enough files so that the entire back-up takes 15 minutes to complete
and you start the back-up at 23:50, the resultant files on the FTP server will be split into two folders because
of the date change. So, if you schedule a backup between 23:45 and 00:00, the software will automatically move the
start time to 00:00.

Although this code is still in development, the code on which it is based has been running reliably in a production
environment for over a year. The only new features not tested are the SSL/TLS Encrypted file and Email transfers.
You should be able to depend on this code for backups and email messages in unencrypted mode. However, we would
appreciate your testing the code using SSL/TLS, and reporting the results back to us.

The utility does not currently use the Windows Volume Shadow Copy service while creating the Web site archives.
We have only experienced one issue because of this, when copying a WordPress site, while the WordFence security
plug-in was scanning the site. By adjusting the Scheduler time to avoid activation during the time the WordFence
plug-in scans, we were able to avoid any conflicts. The utility has been archiving and backing up over 10 large
IIS Web sites every day without issue. Regardless, we don't recommend copying and archiving databases directly,
until the Volume Shadow Copy code has been implemented in the application. For the time being, use your database
maintenance program to archive your databases, and copy them to the Database backup folder for the utility to
upload.

Licensed under GNU GENERAL PUBLIC LICENSE - Version 3, 29 June 2007
