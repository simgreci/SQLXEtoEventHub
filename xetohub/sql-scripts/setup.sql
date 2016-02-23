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

if exists (select name from sys.procedures where name = 'sp_send_xe_to_eventhub')
	drop procedure sp_send_xe_to_eventhub
go
if exists (select name from sys.assemblies where name = 'xetohub')
	drop assembly xetohub
go

create assembly xetohub from 'c:\sqlserver\clr\xetohub.dll' with permission_set = external_access
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