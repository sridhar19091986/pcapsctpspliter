REM: note that the '-sprocs' option is turned on




REM: MySql----------------------------------



dbMetal.exe  -provider=MySql  -database:guangzhou_0410  -server:192.168.4.209  -user:hantele  -password:hantele -namespace:GnRealTimeWork  -dbml:GnFromMySql.dbml  

dbMetal.exe  -provider=MySql  -database:guangzhou_0410   -server:192.168.4.209  -user:hantele  -password:hantele -namespace:GnRealTimeWork  -code:GnFromMySql.cs  

@echo


@pause

