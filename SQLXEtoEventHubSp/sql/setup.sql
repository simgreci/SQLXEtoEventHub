/*xetohub database setup script*/

if not exists(select name from sys.databases where name = 'xetohub')
	create database xetohub
go

alter database xetohub set trustworthy on;
create assembly System_Runtime_Serialization from 
'C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Runtime.Serialization.dll'
with permission_set = unsafe;

create assembly xetohub from 'c:\sqlserver\clr\sqlxetoeventhubsp.dll' with permission_set = unsafe;