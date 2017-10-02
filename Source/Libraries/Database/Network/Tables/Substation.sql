CREATE TABLE [dbo].[Substation]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Acronym] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [ParentDivision] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_Substation_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Substation_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_Substation_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Substation_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_Substation_Division] FOREIGN KEY ([ParentDivision]) REFERENCES [Division]([Uid]),
	CONSTRAINT [CK_Substation_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_Substation_Number] CHECK (Number > 0), 
    CONSTRAINT [CK_Substation_Acronym] CHECK (Acronym NOT LIKE '%[^A-Z]%')
)

GO

CREATE INDEX [IX_Substation_Id] ON [dbo].[Substation] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_SubstationUpdated]
    ON [dbo].[Substation]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[Substation] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[Substation] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END