# ServerBackupUtility
Utility Application to Backup Files From A Windows Server to an FTP Server


Dependencies: This Windows application requires the .Net Framework v.4.7.2

You can download the offline installer here:
https://go.microsoft.com/fwlink/?LinkId=863261


Server Backup Utility Installation Instructions
-----------------------------------------------

1) Unzip the ServerBackupUtility.zip file into a temporary folder.
2) Add the relevent entries to the ServerBackupFiles.txt file in the Release folder (see below).
3) Add the relevent entries to the ServerBackupUtility.exe.config file in the Release folder (see below).
4) Switch to the Installation folder and right click on the UtilityInstall.bat file. Select Run as Admin.
5) The batch file will create a system drive \BackupUtility folder and install itself as a Windows App.
6) The batch file will set permissions for Network Service on the folders to run the utility package.
7) Use Task Scheduler to schedule execution of the Server Backup Utility using  Network Service account.


To upgrade an existing version of the utility, run the UtilityUninstall.bat file,
then run the UtilityInstall.bat file. Always run these files in Administrator mode.

Questions or Problems: You can reach me at fwchateau@gmail.com


ServerBackupFiles.txt
---------------------

Each line should contain a folder path ending with a file name, or a pattern match to a set of file names.
File match is recursive below the upload folder. This can be changed in the UploadService code.

Pattern Match
---------------------
\* = wildcard match (matches anything)

? = character match (matches one character)

C:\csvn\data\dumps\console\*    <-- Back-up all files in the \console folder

C:\csvn\data\dumps\identity\identity.exe    <-- Back-up the "identity.exe" file in the \identity folder

C:\csvn\data\dumps\maps\*.zip    <-- Back-up all files in the \maps folder with the file extension ".zip"

C:\csvn\data\dumps\samples\logs.*    <-- Back-up all files in the \samples folder with the file name "logs"
                                          and any extension

C:\csvn\data\dumps\database\log??.zip    <-- Back-up all files in the \database folder with the file name
                                              "log[any character][any character].zip"


ServerBackupService.exe.config
------------------------------

add key="FolderPaths" value="D:\Webs|D:\Media\Video"     <--- Folders separated by a pipe character,
                                                                 containing multiple files which should be
                                                                 archived and backed up as a single file

add key="ArchivePath" value="D:\Backups"     <--- Folder to place the above archived files for backup
                                                     (All files in this folder will be backed up)

add key="DatabasePath" value="D:\SqlServerDataFiles\Backup"     <--- Folder to place database archives
                                                                        for backup (We do not recommend
                                                                        archiving database files with this
                                                                        utility until we implement Volume
                                                                        Shadow Copy. Use your database
                                                                        maintenance utility to archive
												                                                your databases and place the
                                                                        archives in this folder)

add key="FtpUrl" value="online-server.com"    <--- Url of the FTP server

add key="FtpUserName" value="backupUser"    <--- User name for the FTP server account

add key="FtpPassword" value="password"    <--- Password for the FTP server user

add key="FtpMode" value="active"     <--- Select active or passive file transfer

add key="FtpPort" value="21"     <--- Usually port 21 for active and port 990 for passive

add key="FtpSsl" value="false"     <--- Select encrypted or unencrypted file transfer

add key="SmtpService" value="true"    <--- Use the SMTP mail service to send log notifications

add key="SmptUserName" value="services"    <--- SMTP user account name

add key="SmptPassword" value="password"    <--- SMTP user account password

add key="SmptHost" value="mail-server.com"     <--- Domain name of the SMTP server

add key="SmptPort" value="25"    <--- SMTP server access port - Usually port 25

add key="SmtpSsl" value="false"    <--- Select encrypted or unencrypted file transfer

add key="SmtpSender" value="services@mail-server.com"    <--- Email sender address

add key="SmtpRecipient" value="admin@mail-server.com"    <--- Email recipient address


-----------------------

Note: Because of the way we use dates as back-up folders on the FTP server, it is not a good idea to set the
      start time too close before Midnight. For example, if you have enough files so that the entire back-up
      takes 20 minutes to complete and you start the back-up at 23:50, the resultant files on the FTP server
      will be split into two folders because of the date change. So, if you start your back-up before Midnight,
      give it enough time to complete before Midnight. Otherwise, start it after Midnight and no problem.


Licensed under GNU GENERAL PUBLIC LICENSE - Version 3, 29 June 2007

