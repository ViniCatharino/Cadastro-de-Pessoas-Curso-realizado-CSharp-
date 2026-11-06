-- 01_criar_banco.sql
IF DB_ID('CadastroPessoas') IS NULL
BEGIN
  CREATE DATABASE CadastroPessoas;
END
GO

USE CadastroPessoas;
GO

IF OBJECT_ID('dbo.Clientes', 'U') IS NULL
BEGIN
  CREATE TABLE dbo.Pessoas(Id         Integer Identity(1,1) PRIMARY KEY
                          ,Nome       Varchar(100) NOT NULL
                          ,Email      Varchar(150)
                          ,CriadoEm   DateTime DEFAULT GETDATE()
                          ,AlteradoEm DateTime NULL);
END
GO



