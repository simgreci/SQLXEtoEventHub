/*xetohub database setup script*/
use master
go

if not exists(select name from sys.databases where name = 'xetohub')
	create database xetohub
go

alter database xetohub set trustworthy on
go

use xetohub
go

create assembly [System.Runtime.Serialization] from 
'C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Runtime.Serialization.dll'
with permission_set = unsafe
go

create assembly xetohub from 'c:\sqlserver\clr\sqlxetoeventhubsp.dll' with permission_set = unsafe
go