CREATE TABLE [dbo].[Switch]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [ParentSubstation] UNIQUEIDENTIFIER NULL, 
    [ParentTransmissionLine] UNIQUEIDENTIFIER NULL, 
    [OutputKey] NVARCHAR(200) NOT NULL DEFAULT 'Undefined', 
    [NormalState] UNIQUEIDENTIFIER NOT NULL, 
    [FromNode] UNIQUEIDENTIFIER NOT NULL, 
    [ToNode] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_Switch_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Switch_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_Switch_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Switch_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_Switch_Substation] FOREIGN KEY ([ParentSubstation]) REFERENCES [Substation]([Uid]), 
    CONSTRAINT [FK_Switch_TransmissionLine] FOREIGN KEY ([ParentTransmissionLine]) REFERENCES [TransmissionLine]([Uid]), 
    CONSTRAINT [FK_Switch_SwitchingDeviceNormalState] FOREIGN KEY ([NormalState]) REFERENCES [SwitchingDeviceNormalState]([Uid]), 
    CONSTRAINT [FK_Switch_FromNode] FOREIGN KEY ([FromNode]) REFERENCES [Node]([Uid]), 
    CONSTRAINT [FK_Switch_ToNode] FOREIGN KEY ([ToNode]) REFERENCES [Node]([Uid]),
	CONSTRAINT [CK_Switch_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_Switch_Number] CHECK (Number > 0)
)

GO

CREATE INDEX [IX_Switch_Id] ON [dbo].[Switch] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_SwitchUpdated]
    ON [dbo].[Switch]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[Switch] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[Switch] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END