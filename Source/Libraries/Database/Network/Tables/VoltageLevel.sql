CREATE TABLE [dbo].[VoltageLevel]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [BaseKv] FLOAT NOT NULL DEFAULT 500.0, 
    [ParentModel] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_VoltageLevel_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_VoltageLevel_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_VoltageLevel_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_VoltageLevel_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_VoltageLevel_Model] FOREIGN KEY ([ParentModel]) REFERENCES [Model]([Uid]) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT [CK_VoltageLevel_Id] CHECK (Id > 0),
	CONSTRAINT [CK_VoltageLevel_BaseKv] CHECK (BaseKv > 0 AND BaseKv <= 1000)
)

GO

CREATE INDEX [IX_VoltageLevel_Id] ON [dbo].[VoltageLevel] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_VoltageLevelUpdated]
    ON [dbo].[VoltageLevel]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[VoltageLevel] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[VoltageLevel] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END