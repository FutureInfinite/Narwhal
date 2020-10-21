@echo off

title Narwhal - Front

if not exist "logs" mkdir logs
if not exist "temp" mkdir temp

copy /Y %~dp0\nginx.conf.template temp\nginx.conf
copy /Y %~dp0\nginx-1.18.0\conf\mime.types temp\mime.types

%~dp0\fart199b_win64\fart.exe temp\nginx.conf "${SERVICE_HOST}" "127.0.0.1"
%~dp0\fart199b_win64\fart.exe temp\nginx.conf "${SERVICE_PORT}" "6161"
%~dp0\fart199b_win64\fart.exe temp\nginx.conf "${APP_HOST}" "127.0.0.1"
%~dp0\fart199b_win64\fart.exe temp\nginx.conf "${APP_PORT}" "6162"

echo Running nginx...

%~dp0\nginx-1.18.0\nginx.exe -g "daemon off; master_process off;" -c temp\nginx.conf