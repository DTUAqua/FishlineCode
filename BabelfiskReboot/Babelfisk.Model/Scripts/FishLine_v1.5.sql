USE [FishLine]
GO



/****** Object:  Table [dbo].[Report]    Script Date: 07/02/2015 14:51:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Report](
	[reportId] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](500) NOT NULL,
	[description] [nvarchar](max) NULL,
	[type] [nvarchar](150) NOT NULL,
	[data] [nvarchar](max) NULL,
	[isAvailableOffline] [bit] NOT NULL
 CONSTRAINT [PK_Report] PRIMARY KEY CLUSTERED 
(
	[reportId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



/****** Object:  Table [dbo].[ReportingTreeNode]    Script Date: 07/02/2015 14:53:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReportingTreeNode](
	[reportingTreeNodeId] [int] IDENTITY(1,1) NOT NULL,
	[parentTreeNodeId] [int] NULL,
	[name] [nvarchar](500) NOT NULL,
	[description] [nvarchar](max) NULL,
 CONSTRAINT [PK_ReportingTreeNode] PRIMARY KEY CLUSTERED 
(
	[reportingTreeNodeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ReportingTreeNode]  WITH CHECK ADD  CONSTRAINT [FK_ReportingTreeNode_ReportingTreeNode] FOREIGN KEY([parentTreeNodeId])
REFERENCES [dbo].[ReportingTreeNode] ([reportingTreeNodeId])
GO

ALTER TABLE [dbo].[ReportingTreeNode] CHECK CONSTRAINT [FK_ReportingTreeNode_ReportingTreeNode]
GO






/****** Object:  Table [dbo].[R_ReportingTreeNodeReport]    Script Date: 07/02/2015 14:53:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[R_ReportingTreeNodeReport](
	[reportingTreeNodeId] [int] NOT NULL,
	[reportId] [int] NOT NULL,
 CONSTRAINT [PK_R_ReportingTreeNodeReport] PRIMARY KEY CLUSTERED 
(
	[reportingTreeNodeId] ASC,
	[reportId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[R_ReportingTreeNodeReport]  WITH CHECK ADD  CONSTRAINT [FK_R_ReportingTreeNodeReport_Report] FOREIGN KEY([reportId])
REFERENCES [dbo].[Report] ([reportId])
GO

ALTER TABLE [dbo].[R_ReportingTreeNodeReport] CHECK CONSTRAINT [FK_R_ReportingTreeNodeReport_Report]
GO

ALTER TABLE [dbo].[R_ReportingTreeNodeReport]  WITH CHECK ADD  CONSTRAINT [FK_R_ReportingTreeNodeReport_ReportingTreeNode] FOREIGN KEY([reportingTreeNodeId])
REFERENCES [dbo].[ReportingTreeNode] ([reportingTreeNodeId])
GO

ALTER TABLE [dbo].[R_ReportingTreeNodeReport] CHECK CONSTRAINT [FK_R_ReportingTreeNodeReport_ReportingTreeNode]
GO


---- Insert default root nodes
INSERT INTO [FishLine].[dbo].[ReportingTreeNode] ([parentTreeNodeId] ,[name] ,[description])
     VALUES (NULL , 'Rapporter', 'Online rapporter')
GO



ALTER TABLE [dbo].[Report]
ADD [outputFormat] [nvarchar] (150) NULL
GO



--- Missing to be released on test server (BELOW)

USE [FishLine]
GO


---- Create new otolith image files table
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AnimalFile](
	[animalFileId] [int] IDENTITY(1,1) NOT NULL,
	[animalId] [int] NOT NULL,
	[filePath] [nvarchar](1000) NOT NULL,
	[fileType] [nvarchar](100) NOT NULL,
	[autoAdded] [bit] NOT NULL,
 CONSTRAINT [PK_AnimalFile] PRIMARY KEY CLUSTERED 
(
	[animalFileId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[AnimalFile]  WITH CHECK ADD  CONSTRAINT [FK_AnimalFile_Animal] FOREIGN KEY([animalId])
REFERENCES [dbo].[Animal] ([animalId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AnimalFile] CHECK CONSTRAINT [FK_AnimalFile_Animal]
GO



-- Add stored proceudre for running a job

-- =============================================
-- Author:		TVdP
-- Create date: 20090706
-- Description:	Starts a SQLAgent Job and waits for it to finish or until a specified wait period elapsed
-- @result:	1 -> OK
--			0 -> still running after maxwaitmins
-- =============================================
CREATE procedure [dbo].[StartAgentJobAndWait](@job nvarchar(128), @maxwaitmins int = 5) --, @result int output)
as begin

set NOCOUNT ON;
set XACT_ABORT ON;

	BEGIN TRY

	declare @running as int
	declare @seccount as int
	declare @maxseccount as int
	declare @start_job as bigint
	declare @run_status as int

	set @start_job = cast(convert(varchar, getdate(), 112) as bigint) * 1000000 + datepart(hour, getdate()) * 10000 + datepart(minute, getdate()) * 100 + datepart(second, getdate())

	set @maxseccount = 60*@maxwaitmins
	set @seccount = 0
	set @running = 0

	declare @job_owner sysname
	declare @job_id UNIQUEIDENTIFIER

	set @job_owner = SUSER_SNAME()

	-- get job id
	select @job_id=job_id
	from msdb.dbo.sysjobs sj
	where sj.name=@job

	-- invalid job name then exit with an error
	if @job_id is null
		RAISERROR (N'Unknown job: %s.', 16, 1, @job)

	-- output from stored procedure xp_sqlagent_enum_jobs is captured in the following table
	declare @xp_results TABLE ( job_id                UNIQUEIDENTIFIER NOT NULL,
								last_run_date         INT              NOT NULL,
								last_run_time         INT              NOT NULL,
								next_run_date         INT              NOT NULL,
								next_run_time         INT              NOT NULL,
								next_run_schedule_id  INT              NOT NULL,
								requested_to_run      INT              NOT NULL, -- BOOL
								request_source        INT              NOT NULL,
								request_source_id     sysname          COLLATE database_default NULL,
								running               INT              NOT NULL, -- BOOL
								current_step          INT              NOT NULL,
								current_retry_attempt INT              NOT NULL,
								job_state             INT              NOT NULL)

	-- start the job
	declare @r as int
	exec @r = msdb..sp_start_job @job

	-- quit if unable to start
	if @r<>0
		RAISERROR (N'Could not start job: %s.', 16, 2, @job)

	-- start with an initial delay to allow the job to appear in the job list (maybe I am missing something ?)
	WAITFOR DELAY '0:0:01';
	set @seccount = 1

	-- check job run state
	insert into @xp_results
	execute master.dbo.xp_sqlagent_enum_jobs 1, @job_owner, @job_id

	set @running= (SELECT top 1 running from @xp_results)

	while @running<>0 and @seccount < @maxseccount
	begin
		WAITFOR DELAY '0:0:01';
		set @seccount = @seccount + 1

		delete from @xp_results

		insert into @xp_results
		execute master.dbo.xp_sqlagent_enum_jobs 1, @job_owner, @job_id

		set @running= (SELECT top 1 running from @xp_results)
	end

	-- result: not ok (=1) if still running

	if @running <> 0 begin
		-- still running
		return 0
	end
	else begin

		-- did it finish ok ?
		set @run_status = 0

		select @run_status=run_status
		from msdb.dbo.sysjobhistory
		where job_id=@job_id
		  and cast(run_date as bigint) * 1000000 + run_time >= @start_job

		if @run_status=1
			return 1  --finished ok
		else  --error
			RAISERROR (N'job %s did not finish successfully.', 16, 2, @job)

	end

	END TRY
	BEGIN CATCH

    DECLARE
        @ErrorMessage    NVARCHAR(4000),
        @ErrorNumber     INT,
        @ErrorSeverity   INT,
        @ErrorState      INT,
        @ErrorLine       INT,
        @ErrorProcedure  NVARCHAR(200);

    SELECT
        @ErrorNumber = ERROR_NUMBER(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE(),
        @ErrorLine = ERROR_LINE(),
        @ErrorProcedure = ISNULL(ERROR_PROCEDURE(), '-');

    SELECT @ErrorMessage =
        N'Error %d, Level %d, State %d, Procedure %s, Line %d, ' +
            'Message: '+ ERROR_MESSAGE();

    RAISERROR
        (
        @ErrorMessage,
        @ErrorSeverity,
        1,
        @ErrorNumber,    -- original error number.
        @ErrorSeverity,  -- original error severity.
        @ErrorState,     -- original error state.
        @ErrorProcedure, -- original error procedure name.
        @ErrorLine       -- original error line number.
        );

	END CATCH

end;
GO



-- Add right permission for executing stored proceudre
-- Make sure to give the "fishlineuser" access to read master and msdb databases and SQLAgentOperatorRole for msdb database as well.
USE [FishLine]
GO
GRANT EXECUTE ON [dbo].[StartAgentJobAndWait] TO fishlineuser;

USE [master]
GO
GRANT EXECUTE ON [dbo].[xp_sqlagent_enum_jobs] TO fishlineuser;





-- Make changes to FishLineDW

USE [FishLineDW]
GO


-- Add new R_AnimalPictureReference table

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[R_AnimalPictureReference](
	[R_animalPictureReferenceId] [int] IDENTITY(1,1) NOT NULL,
	[animalId] [int] NOT NULL,
	[pictureReference] [nvarchar](1000) NOT NULL,
 CONSTRAINT [PK_R_AnimalPictureReference] PRIMARY KEY CLUSTERED 
(
	[R_animalPictureReferenceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[R_AnimalPictureReference]  WITH NOCHECK ADD  CONSTRAINT [FK_R_AnimalPictureReference_Animal] FOREIGN KEY([animalId])
REFERENCES [dbo].[Animal] ([animalId])
GO

ALTER TABLE [dbo].[R_AnimalPictureReference] CHECK CONSTRAINT [FK_R_AnimalPictureReference_Animal]
GO





--- Insert job for searching for images. Remember to refactor login name and path to application

USE [msdb]
GO

/****** Object:  Job [FishLineNightlyFileSynchronizer]    Script Date: 12/08/2015 11:23:11 ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]]    Script Date: 12/08/2015 11:23:11 ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'FishLineFileSynchronizer', 
		@enabled=0, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'No description available.', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'GS60-MDU\Administrator', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Run FIleSynchronizer]    Script Date: 12/08/2015 11:23:12 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Run FIleSynchronizer', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'CmdExec', 
		@command=N'C:\Program Files (x86)\AnchorLab\FileSynchronizer\Babelfisk.FileSynchronizer.exe', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:

GO