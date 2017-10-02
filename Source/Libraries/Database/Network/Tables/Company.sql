CREATE TABLE [dbo].[Company]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Acronym] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [ParentModel] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_Company_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Company_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_Company_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Company_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_Company_Model] FOREIGN KEY ([ParentModel]) REFERENCES [Model]([Uid]) ON DELETE CASCADE ON UPDATE CASCADE,    
	CONSTRAINT [CK_Company_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_Company_Number] CHECK (Number > 0), 
    CONSTRAINT [CK_Company_Acronym] CHECK (Acronym NOT LIKE '%[^A-Z]%')
)

GO

CREATE INDEX [IX_Company_Id] ON [dbo].[Company] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_CompanyUpdate]
    ON [dbo].[Company]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[Company] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[Company] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END