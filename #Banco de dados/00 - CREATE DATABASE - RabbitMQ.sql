USE [master] 

IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'RabbitMQ')
BEGIN
	CREATE DATABASE RabbitMQ
END