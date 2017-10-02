CREATE TABLE [dbo].[TapConfiguration]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Acronym] NVARCHAR(50) NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [LowerBounds] FLOAT NOT NULL DEFAULT 0.92, 
    [UpperBounds] FLOAT NOT NULL DEFAULT 1.08, 
    [LowerPositionBounds] INT NOT NULL DEFAULT -16, 
    [UpperPositionBounds] INT NOT NULL DEFAULT 16, 
    [NominalPosition] INT NOT NULL DEFAULT 0, 
    [ParentModel] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_TapConfiguration_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_TapConfiguration_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_TapConfiguration_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_TapConfiguration_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_TapConfiguration_Model] FOREIGN KEY ([ParentModel]) REFERENCES [Model]([Uid]) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT [CK_TapConfiguration_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_TapConfiguration_Acronym] CHECK (Acronym NOT LIKE '%[^A-Z]%')
)

GO

CREATE INDEX [IX_TapConfiguration_Id] ON [dbo].[TapConfiguration] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_TapConfigurationUpdated]
    ON [dbo].[TapConfiguration]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[TapConfiguration] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[TapConfiguration] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END