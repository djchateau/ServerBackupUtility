
ServerBackupUtility
-------------------

Utility Application to Backup Files from a Windows Server to an FTP Server

Dependencies: This Windows application requires the .Net Framework v.4.7.2 run-time to be installed on the server. The installer program will install this run-time version on the server during the install, but you will need to install the developer pack version on your machine, if you want to run the Visual Studio solution. In addition, you will need to install the Advanced Installer Visual Studio extension if you want to open the installer project.

You can download the .Net Framework v.4.7.2 Visual Studio developer pack here: \
    https://go.microsoft.com/fwlink/?LinkId=863261 \

A gracious thank you goes out to Caphyon Ltd. for providing us with a copy of their Advanced Installer program. If you need an installer application, check it out. It's the best one I've found. \
    https://www.advancedinstaller.com
	
	
Server Backup Utility Installation Instructions
-----------------------------------------------
	
If All Else Fails, Read The Directions...

1) Run the ServerBackupUtility-Installer.exe on your Windows server.

2) Open the application folder, which can be found at \Program Files\ServerBackupUtility.

3) Edit the DirectBackupPaths.txt file to point to the files you wish to directly back up. The utility will transfer each of these files directly to the FTP server and place them in a folder labeled with the current date.

4) Edit the ServerBackupUtility.exe.config file to point to a folder that contains sub-folders which you wish to archive. For example, your Web sites can all be located under a root folder \Webs. The root folder name should be placed in the config file. The utility will archive all the sub-folders (the Web sites) under the root folder, and place them in a temporary folder (also configurable in the config file) for transfer to the FTP server. You can add as many root folders as you wish, but the utility always looks in the root folder for sub-folders to archive. As this time, you cannot archive a root folder directly -- Only sub-folders below the root.

5) Enter the names of the root folders in the config file, separated by a pipe character. Also, enter a name for the temporary folder that holds the archived files for transfer to the FTP folder.

6) Add a folder name to the config file that contains your database archive files. The utility will directly transfer all files in that folder to the FTP server and place them in the same folder on the FTP server, as all the files above.

7) Use the Delete key switch in the config file if you want to clear out these files from the Windows server after they are transferred to the FTP server.


Questions, Suggestions or Feature Requests
------------------------------------------

You can reach me at fwchateau@gmail.com. Add Server Backup Utility to the subject line. I filter this email address to eliminate SPAM. If you do not add the phrase Server Backup Utility to the subject line, I may not see your email.


DirectBackupPaths.txt File
--------------------------

Each line should contain a folder path ending with a file name, or a pattern match to a set of file names. The match pattern is recursive below the target folder. This can be changed in the Upload Service code using SearchOptions.TopDirectoriesOnly.

\* = wildcard match (matches anything) \
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

- List of root folders separated by pipe characters, containing target sub-folders to be archived for backup. You cannot archive a root folder. Place your files in a sub-folder and add the root folder here. You can have as many sub-folders as you wish. \
	add key="ArchivePaths" value="D:\Webs|D:\Media"

- Temporary Folder in which to place archived files for backup. All files in this folder will be transfered to the FTP server. You can manually place files in this folder. The application will place the files it archived here. \
	add key="BackupPath" value="D:\Backups"
   
- Folder to place the database archives. We do not recommend archiving database files with this utility. Use your database maintenance utility to archive your databases and place the archives in this folder. \
	add key="DatabasePath" value="D:\SqlServerDataFiles\Backup"

- You can use this setting so old backup files don't accumulate on the Windows server. It deletes the archived files from the Windows server after they have been transferred to the FTP server. \
    add key="DeleteFiles" value="true"

- Url of the FTP server \
	add key="FtpUrl" value="online-server.com"

- User Name for the FTP server account \
	add key="FtpUserName" value="backupUser"

- Password for the FTP server user \
	add key="FtpPassword" value="password"

- Select Active or Passive FTP file transfer. \
	add key="FtpMode" value="active"

- Usually port 21 here for Active transfers and port 990 for Passive transfers, but not necessarily. Check the configuration on your FTP server. \
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

Permissions can be a little confusing because of all the folders involved in backing up the files. The easiest way to get things up and running is to install the utility using the Local System account, which the installer does by default. All folders and files you will be accessing include the Local System account permissions by default, but in the unlikely event you get hacked, I don't want to be responsible for it, so I'm adding this warning. You may want to change the permissions to Local Service or Network Service, which can have a stronger security effect on the server. To do this you must add the Local Service or Network Service accounts to the Backup Scheduler service and to any folders and files the service touches, or you will get access errors. Don't try this if you don't know what you are doing.


Miscellaneous Notes
---------------------

If you change settings in the config file while the utility is running, the Backup Scheduler will automatically restart itself to pick up the new settings. You can also restart the Backup Scheduler manually by running the RestartScheduler.bat file.

Because of the way we use dates as back-up folders on the FTP server, it is not a good idea to set the start time too close to Midnight. For example, if you have enough files so that the entire back-up takes 15 minutes to complete and you start the back-up at 23:50, the resultant files on the FTP server will be split into two folders because of the date change. So, if you schedule a backup between 23:45 and 00:00, the software will automatically move the start time to 00:00.

Although this code is still in development, the code on which it is based has been running reliably in a production environment for over a year. The only new features not tested are the SSL/TLS Encrypted file and Email transfers. You should be able to depend on this code for backups and email messages in unencrypted mode. However, we would appreciate your testing the code using SSL/TLS, and reporting the results back to us.

The utility does not currently use the Windows Volume Shadow Copy service while creating the Web site archives. We have only experienced one issue because of this, when copying a WordPress site, while the WordFence security plug-in was scanning the site. By adjusting the Backup Scheduler time to avoid activation during the time the WordFence plug-in scans, we were able to avoid any conflicts. The utility has been archiving and backing up over 10 large IIS Web sites every day without issue. Regardless, we don't recommend archiving databases with this utility, until the Volume Shadow Copy code has been implemented in the application. For the time being, use your database maintenance program to archive your databases, and copy them to the Database backup folder for the utility to upload.

Licensed under GNU GENERAL PUBLIC LICENSE - Version 3, 29 June 2007

