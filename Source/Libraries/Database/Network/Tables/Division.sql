CREATE TABLE [dbo].[Division]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Acronym] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [ParentCompany] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_Division_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Division_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_Division_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Division_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_Division_Company] FOREIGN KEY ([ParentCompany]) REFERENCES [Company]([Uid]) ON DELETE CASCADE ON UPDATE CASCADE, 
    CONSTRAINT [CK_Division_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_Division_Number] CHECK (Number > 0), 
    CONSTRAINT [CK_Division_Acronym] CHECK (Acronym NOT LIKE '%[^A-Z]%')
)

GO

CREATE INDEX [IX_Division_Id] ON [dbo].[Division] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_DivisionUpdate]
    ON [dbo].[Division]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[Division] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[Division] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END