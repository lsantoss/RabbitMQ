USE [RabbitMQ]

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='Reversal') 
BEGIN
	CREATE TABLE [dbo].[Reversal](
		[Id] [uniqueidentifier] NOT NULL,
		[PaymentId] [uniqueidentifier] NOT NULL,
		[Date] [smalldatetime] NOT NULL,
		[CreationDate] [smalldatetime] NOT NULL,

	CONSTRAINT [PK_Reversal] PRIMARY KEY CLUSTERED([Id] ASC)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) 
	ON [PRIMARY]

	ALTER TABLE [dbo].[Reversal]  WITH CHECK ADD  CONSTRAINT [FK_Reversal_Payment] FOREIGN KEY([PaymentId])
	REFERENCES [dbo].[Payment] ([Id])

	ALTER TABLE [dbo].[Reversal] CHECK CONSTRAINT [FK_Reversal_Payment]
END