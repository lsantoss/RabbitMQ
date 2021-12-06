USE [RabbitMQ] 

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='LogFila') 
BEGIN
    CREATE TABLE [dbo].[LogFila] (
        [Id] [bigint] IDENTITY(1,1) NOT NULL,
        [IdPagamento] [uniqueidentifier] NOT NULL,
        [Worker] [nvarchar](50) NOT NULL,
        [Fila] [nvarchar](50) NOT NULL,
        [Mensagem] [text] NOT NULL,
        [DataHora] [smalldatetime] NOT NULL,
        [Sucesso] [bit] NOT NULL,
        [NumeroTentativas] [tinyint] NOT NULL,
        [Erro] [text] NULL,
        CONSTRAINT [PK_LogFila] PRIMARY KEY CLUSTERED 
    (
	    [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END