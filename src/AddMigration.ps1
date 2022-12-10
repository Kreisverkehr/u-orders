param (
	[Parameter(Mandatory)]
	[string]
	$MigrationName
)

Add-Migration $MigrationName -Context UOrdersDbContext -StartupProject UOrders.Service -Project UOrders.EFModel.SqlServer -Args "/Db:Provider=mssql /Db:Host=localhost /Db:Port=51433 /Db:User=sa /Db:Password=yourStrong(!)Password"
Add-Migration $MigrationName -Context UOrdersDbContext -StartupProject UOrders.Service -Project UOrders.EFModel.Mysql -Args "/Db:Provider=mysql /Db:Host=localhost /Db:Port=53306 /Db:DbName=dev /Db:User=dev /Db:Password=dev"
Add-Migration $MigrationName -Context UOrdersDbContext -StartupProject UOrders.Service -Project UOrders.EFModel.Postgres -Args "/Db:Provider=postgres /Db:Host=localhost /Db:Port=55432 /Db:DbName=postgres /Db:User=postgres /Db:Password=mysecretpassword"
