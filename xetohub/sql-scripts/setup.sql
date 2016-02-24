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
if exists (select name from sys.assemblies where name = 'xetohub')
	drop assembly xetohub
go

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

USE [master];
GO