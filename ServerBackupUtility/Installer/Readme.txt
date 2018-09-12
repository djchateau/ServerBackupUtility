
ServerBackupUtility
-------------------

Utility Application to Backup Files from a Windows Server to an FTP Server

Dependencies: This Windows application requires the .Net Framework v.4.7.2 to be installed on the server.

You can download the v.4.7.2 run-time here:
https://www.microsoft.com/net/download/thank-you/net472

You can download the v.4.7.2 developer version here:
https://go.microsoft.com/fwlink/?LinkId=863261


Server Backup Utility Installation Instructions
-----------------------------------------------
If all else fails, read the directions...

1) Unzip the ServerBackupUtility.zip file into a temporary folder.
2) Add the relevent entries to the ServerBackupFiles.txt file in the Release folder (see below).
3) Add the relevent entries to the ServerBackupUtility.exe.config file in the Release folder (see below).
4) Switch to the Installation folder and right click on the UtilityInstall.bat file. Select Run as Administrator.
5) The batch file will create a system drive \BackupUtility folder and install itself as a Windows Application.
6) The batch file will set permissions for Network Servcie on the folder and sub-folder to run the utility package.
7) Use Task Scheduler to schedule execution of the Server Backup Utility using the Network Service account.


To upgrade an existing version of the utility, run the UtilityUninstall.bat file,
then run the UtilityInstall.bat file. Always run these files in Administrator mode.

Questions or Problems: You can reach me at fwchateau@gmail.com (Place Server Backup Utility in the subject line.)


ServerBackupFiles.txt
---------------------

Each line should contain a folder path ending with a file name, or a pattern match to a set of file names.
The file match pattern is recursive below the target folder. This can be changed in the UploadService code
using SearchOptions.TopDirectoriesOnly.

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


ServerBackupService.exe.config
------------------------------

- List of folders separated by pipe characters, containing target sub-folders with multiple files,
  all of which will be archived and backed up as a single zip file \
	add key="FolderPaths" value="D:\Webs|D:\Media\Video"

- Folder to store the above archived files for backup (All files in this folder will be backed up.) \
	add key="ArchivePath" value="D:\Backups"
   
- Folder to store the database archives (We do not recommend archiving database files with this utility.
  Use your database maintenance utility to archive your databases and place the backups in this folder.) \
	add key="DatabasePath" value="D:\SqlServerDataFiles\Backup"

- Scheduler Mode (Select Clock for backup at a regularly scheduled time or Interval for repetitive tasks) \
    add key="Mode" value="clock"

- Clock Time in 24 Hour Format \
    add key="Clock" value="06:45"

- Interval Time in Minutes \
    add key="Interval" value="60"

- Url of the FTP server \
	add key="FtpUrl" value="online-server.com"

- User name for the FTP server account \
	add key="FtpUserName" value="backupUser"

- Password for the FTP server user \
	add key="FtpPassword" value="password"

- Select active or passive file transfer \
	add key="FtpMode" value="active"

- Usually port 21 for active and port 990 for passive but not necessarily \
	add key="FtpPort" value="21"

- Select encrypted or unencrypted file transfer \
	add key="FtpSsl" value="false"

- Use the SMTP mail service to send log notifications \
	add key="SmtpService" value="true"

- SMTP user account name \
	add key="SmptUserName" value="services"

- SMTP user account password \
	add key="SmptPassword" value="password"

- Domain name of the SMTP server \
	add key="SmptHost" value="mail-server.com"

- SMTP server access port - Usually port 25 for unencrypted transfer \
	add key="SmptPort" value="25"

- Select encrypted or unencrypted file transfer \
	add key="SmtpSsl" value="false"

- Email sender address \
	add key="SmtpSender" value="services@mail-server.com"

- Email recipient address \
	add key="SmtpRecipient" value="admin@mail-server.com"

Permissions
---------------------

Permissions can be a little confusing because of all the folders involved in backing up the files. The easiest way to get things up
and running is to install the utility using the System account. All folders and files you will be accessing include System permissions
by default, but since, in the unlikely event that you get hacked, I don't want to be responsible for that, so I install the utility as
a Local Service, which has very little permissions assigned to it, but enough to enable the service to do its work. The consequences
of this is you must add the Local Service account to any folders and files the service touches or you will get an access error.

You can install the service using the System account by removing the text [obj= "NT AUTHORITY\LOCAL SERVICE" password= ""] from the
UtilityInstall batch file and you're good to go. If you have trouble installing the utility using the System account or getting it
to work as a Local Service, write me and I'll walk you through it.


Miscellaneous Notes
---------------------

Because of the way we use dates as back-up folders on the FTP server, it is not a good idea to set the start time too close before
Midnight. For example, if you have enough files so that the entire back-up takes 20 minutes to complete and you start the back-up at 23:50,
the resultant files on the FTP server will be split into two folders because of the date change. So, if you schedule a backup between 23:45
and 00:00, the software will automatically move the start time to 00:00.

Although this code is still in development, the code on which it is based has been running reliably in a production environment for the past
year. The only new features currently being tested are the SSL encrypted file and email transfers. You should be able to depend on this code
for backups and email messages. However, we would appreciate your testing the code using SSL, and reporting the results back to us.

The utility does not currently use the Windows Volume Shadow Copy service while creating the Web site archives. We have only experienced one
issue because of this, when copying a WordPress site, while the WordFence security plug-in was running. By adjusting the Scheduler to avoid
activation during the time the WordFence plug-in runs, we were able to avoid any conflicts. The utility has been copying over 10 large IIS
Web sites every day without issue. Regardless, we don't recommend copying and archiving databases directly, until the Volume Shadow Copy code
has been implemented in the application. For the time being, use your database maintenance program to copy and archive your databases, and
place them in a backup folder for the utility to upload.

Licensed under GNU GENERAL PUBLIC LICENSE - Version 3, 29 June 2007
