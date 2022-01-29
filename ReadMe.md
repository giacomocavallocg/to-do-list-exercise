# ToDo List API

ToDo List Api is a simple Api to manage "to do lists". The API does not require a registration and there is no concept of a user.

Through the API it is possible to create lists and insert elements. Each element hace an expiration date and can be marked as completed or not completed. 

## Configuration

The configuration files is *appsettings.[env].json* where env is the name of the environment setted.

In this file you can specify the database information:
- * DatabaseType *: indicates the type of database to use, allowed values are:
	- * SqlLite *: use a version of sqllite in RAM memory
	- * SqlServer *: use SqlServer
- * DbConnectionString *: Connection to the database (required only for SqlServer)

** Note **: with the use of SqlLite the database is deleted at the end of the execution.

The Api description (OpenApi) will be reachable at:

```
[host]/swagger/index.html.
```



