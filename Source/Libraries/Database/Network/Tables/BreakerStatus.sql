CREATE TABLE [dbo].[BreakerStatus]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [Enabled] BIT NOT NULL DEFAULT 1, 
    [MeasurementKey] NVARCHAR(200) NOT NULL DEFAULT 'Undefined', 
    [Bit] UNIQUEIDENTIFIER NOT NULL, 
    [ParentModel] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_BreakerStatus_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_BreakerStatus_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_BreakerStatus_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_BreakerStatus_BreakerStatusBit] FOREIGN KEY ([Bit]) REFERENCES [BreakerStatusBit]([Uid]) ON UPDATE CASCADE, 
    CONSTRAINT [FK_BreakerStatus_Model] FOREIGN KEY ([ParentModel]) REFERENCES [Model]([Uid]) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT [CK_BreakerStatus_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_BreakerStatus_Number] CHECK (Number > 0), 
)

GO

CREATE INDEX [IX_BreakerStatus_Id] ON [dbo].[BreakerStatus] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_BreakerStatusUpdate]
    ON [dbo].[BreakerStatus]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[BreakerStatus] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[BreakerStatus] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END