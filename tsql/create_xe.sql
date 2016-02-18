CREATE EVENT SESSION [GetEvents] ON SERVER 
ADD EVENT sqlserver.error_reported 
ADD TARGET package0.ring_buffer
WITH (STARTUP_STATE=OFF)
GO


