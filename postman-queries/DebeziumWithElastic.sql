CREATE DATABASE DebeziumElasticTest;

USE DebeziumElasticTest
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorLog](
[Id] [int] IDENTITY(1,1) NOT NULL,
[ErrorTitle] [varchar](500) NULL,
[Description] [varchar](500) NULL,
[ErrorDate] [DateTime] NULL,
[ErrorCorelationId] [varchar](500) NULL,
[ErrorLevel] [int] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ErrorLog] ADD CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

USE DebeziumElasticTest
EXEC sys.sp_cdc_enable_db

USE DebeziumElasticTest 
GO 
EXEC sys.sp_cdc_enable_table 
@source_schema = N'dbo', 
@source_name = N'ErrorLog', 
@role_name = NULL, 
@filegroup_name = N'', -- Siz burada kendi tablonuzun FileGroup değerini yazınız eğer herhangi bir FileGroup yaratmadıysanız boş bırakabilirsiniz, 
@supports_net_changes = 0 
GO


INSERT INTO ErrorLog(ErrorTitle,Description,ErrorDate,ErrorCorelationId,ErrorLevel)
     VALUES
           ('Test Error7'
           ,'Test Error Description7'
           ,GETDATE()
           ,NEWID()
           ,5);


--UPDATE ErrorLog SET ErrorLevel = 20 WHERE Id = 1;

Select * from dbo.ErrorLog;

--delete dbo.ErrorLog where Id=2;