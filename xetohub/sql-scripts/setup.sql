/* enable CLR */
EXEC sp_configure 'show advanced options', 1
RECONFIGURE

EXEC sp_configure 'clr enabled', 1
RECONFIGURE

EXEC sp_configure 'show advanced options', 0
RECONFIGURE


/*xetohub database setup script*/
use master
go

/* signature */
CREATE ASYMMETRIC KEY [xetohub_key] FROM EXECUTABLE FILE = 'c:\sqlserver\clr\xetohub.dll'
GO
CREATE LOGIN [xetohub_login] FROM ASYMMETRIC KEY [xetohub_key];
GO
GRANT EXTERNAL ACCESS ASSEMBLY TO [xetohub_login];
GO


if not exists(select name from sys.databases where name = 'xetohub')
	create database xetohub
go

use [xetohub]
go

EXEC sp_changedbowner 'sa';
GO

if exists (select name from sys.procedures where name = 'sp_send_xe_to_eventhub')
	drop procedure sp_send_xe_to_eventhub
go
if exists (select name from sys.procedures where name = 'xetohub_save_or_update_offset')
	drop procedure xetohub_save_or_update_offset
go
if exists (select name from sys.procedures where name = 'xetohub_read_offset')
	drop procedure xetohub_read_offset
go
if exists (select name from sys.assemblies where name = 'xetohub')
	drop assembly xetohub
go
if exists (select name from sys.tables where name = 'xetohub_sessions_file_offset')
	drop table dbo.xetohub_sessions_file_offset
go

create table dbo.xetohub_sessions_file_offset
(
session_name nvarchar(256) not null primary key,
session_file_name nvarchar(260) not null,
session_offset bigint not null,
last_update datetime not null
)

create assembly xetohub from 'c:\sqlserver\clr\xetohub.dll' 
WITH PERMISSION_SET=EXTERNAL_ACCESS;
go

create procedure dbo.sp_send_xe_to_eventhub
(
@trace_name nvarchar(255),
@event_hub_name nvarchar(255),
@service_bus_namespace nvarchar(255),
@policy nvarchar(255),
@policy_key nvarchar(255)
)
as external name xetohub.[xetohub.sql.storedprocedures].[sp_send_xe_to_eventhub]
go

create procedure dbo.xetohub_save_or_update_offset
@session_name nvarchar(256),
@session_file_name nvarchar(260),
@session_offset bigint

as

if exists (select 1 from dbo.xetohub_sessions_file_offset where session_name = @session_name)
	update dbo.xetohub_sessions_file_offset 
	set session_offset = @session_offset,
		last_update = getdate()
	where session_file_name = @session_file_name
else
	insert into dbo.xetohub_sessions_file_offset (session_name, session_file_name, session_offset, last_update)
	values (@session_name, @session_file_name, @session_offset, getdate())
go

create procedure dbo.xetohub_read_offset
@session_name nvarchar(256)

as

select session_name, session_file_name, session_offset, last_update
from dbo.xetohub_sessions_file_offset
where session_name = @session_name
go

USE [master];
GO

