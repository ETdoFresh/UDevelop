set CM_SYNC_USER=ETdoFresh@gmail.com
for /F "tokens=*" %%g IN ('gh auth token') do (SET CM_SYNC_TOKEN=%%g)
cm sync rep:UDevelop@etdofresh_unity@cloud git https://github.com/ETdoFresh/UDevelop --user=%CM_SYNC_USER% --pwd=%CM_SYNC_TOKEN%
pause