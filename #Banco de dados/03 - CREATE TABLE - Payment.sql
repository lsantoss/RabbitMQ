USE [RabbitMQ] 

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='Payment') 
BEGIN
    CREATE TABLE [dbo].[Payment](
		[Id] [uniqueidentifier] NOT NULL,
		[BarCode] [nvarchar](100) NOT NULL,
		[Value] [decimal](18, 2) NOT NULL,
		[Date] [smalldatetime] NOT NULL,
		[ClientName] [nvarchar](100) NOT NULL,
		[ClientEmail] [nvarchar](100) NOT NULL,
		[ClientCellphone] [nvarchar](50) NOT NULL,
		[NotifyByEmail] [tinyint] NOT NULL,
		[NotifyBySMS] [tinyint] NOT NULL,
		[Reversed] [tinyint] NOT NULL,
		[CreationDate] [smalldatetime] NOT NULL,
		[ChangeDate] [smalldatetime] NULL,

	CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) 
	ON [PRIMARY]
END