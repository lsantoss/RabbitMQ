USE [RabbitMQ] 

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='QueueLog') 
BEGIN
    CREATE TABLE [dbo].[QueueLog] (
        [Id] [bigint] IDENTITY(1,1) NOT NULL,
        [PaymentId] [uniqueidentifier] NOT NULL,
        [Worker] [nvarchar](50) NOT NULL,
        [Queue] [nvarchar](50) NOT NULL,
        [Message] [text] NOT NULL,
        [Date] [smalldatetime] NOT NULL,
        [Success] [bit] NOT NULL,
        [NumberAttempts] [tinyint] NOT NULL,
        [Error] [text] NULL,

    CONSTRAINT [PK_QueueLog] PRIMARY KEY CLUSTERED([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY])
	ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END