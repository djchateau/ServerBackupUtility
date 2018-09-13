@ECHO OFF
SC STOP BackupScheduler > nul
PING -n 5 127.0.0.1 > nul
SC START BackupScheduler > nul
PING -n 5 127.0.0.1 > nul
@ECHO ON
