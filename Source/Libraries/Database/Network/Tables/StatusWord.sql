CREATE TABLE [dbo].[StatusWord]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Description] NVARCHAR(200) NULL , 
    [Enabled] BIT NOT NULL DEFAULT 1, 
    [InputKey] NVARCHAR(200) NOT NULL DEFAULT 'Undefined', 
    [ParentModel] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_StatusWord_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_StatusWord_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_StatusWord_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_StatusWord_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_StatusWord_Model] FOREIGN KEY ([ParentModel]) REFERENCES [Model]([Uid]) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT [CK_StatusWord_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_StatusWord_Number] CHECK (Number > 0)
)

GO

CREATE INDEX [IX_StatusWord_Id] ON [dbo].[StatusWord] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_StatusWordUpdated]
    ON [dbo].[StatusWord]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[StatusWord] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[StatusWord] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END