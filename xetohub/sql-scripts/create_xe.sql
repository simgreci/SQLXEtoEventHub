CREATE EVENT SESSION [errors_session] ON SERVER 
ADD EVENT sqlserver.error_reported(
    ACTION(package0.event_sequence,package0.last_error,package0.process_id,sqlos.scheduler_id,sqlos.task_time,sqlos.worker_address,sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.context_info,sqlserver.database_id,sqlserver.database_name,sqlserver.is_system,sqlserver.nt_username,sqlserver.plan_handle,sqlserver.query_hash,sqlserver.query_plan_hash,sqlserver.request_id,sqlserver.server_instance_name,sqlserver.server_principal_name,sqlserver.server_principal_sid,sqlserver.session_id,sqlserver.session_nt_username,sqlserver.sql_text,sqlserver.transaction_id,sqlserver.transaction_sequence,sqlserver.username)) 
ADD TARGET package0.event_file(SET filename=N'H:\SQLServer\XE\errors_session',max_file_size=(20),max_rollover_files=(100))
WITH (STARTUP_STATE=OFF)
GO


SELECT * FROM sys.fn_xe_file_target_read_file('H:\SQLServer\XE\*.xel', NULL, NULL, NULL);