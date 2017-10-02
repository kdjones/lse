CREATE TABLE [dbo].[CircuitBreaker]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [ParentSubstation] UNIQUEIDENTIFIER NULL, 
    [ParentTransmissionLine] UNIQUEIDENTIFIER NULL, 
    [Status] UNIQUEIDENTIFIER NULL, 
    [OutputKey] NVARCHAR(200) NOT NULL DEFAULT 'Undefined', 
    [NormalState] UNIQUEIDENTIFIER NOT NULL, 
    [FromNode] UNIQUEIDENTIFIER NOT NULL, 
    [ToNode] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_CircuitBreaker_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_CircuitBreaker_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_CircuitBreaker_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_CircuitBreaker_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_CircuitBreaker_Substation] FOREIGN KEY ([ParentSubstation]) REFERENCES [Substation]([Uid]) ON DELETE CASCADE ON UPDATE CASCADE, 
    CONSTRAINT [FK_CircuitBreaker_TransmissionLine] FOREIGN KEY ([ParentTransmissionLine]) REFERENCES [TransmissionLine]([Uid]) ON DELETE CASCADE ON UPDATE CASCADE, 
    CONSTRAINT [FK_CircuitBreaker_BreakerStatus] FOREIGN KEY ([Status]) REFERENCES [BreakerStatus]([Uid]) ON DELETE SET NULL ON UPDATE CASCADE, 
    CONSTRAINT [FK_CircuitBreaker_SwitchingDeviceNormalState] FOREIGN KEY ([NormalState]) REFERENCES [SwitchingDeviceNormalState]([Uid]) ON UPDATE CASCADE, 
    CONSTRAINT [FK_CircuitBreaker_FromNode] FOREIGN KEY ([FromNode]) REFERENCES [Node]([Uid]), 
    CONSTRAINT [FK_CircuitBreaker_ToNode] FOREIGN KEY ([ToNode]) REFERENCES [Node]([Uid]),
	CONSTRAINT [CK_CircuitBreaker_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_CircuitBreaker_Number] CHECK (Number > 0), 
)

GO

CREATE INDEX [IX_CircuitBreaker_Id] ON [dbo].[CircuitBreaker] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_CircuitBreakerUpdated]
    ON [dbo].[CircuitBreaker]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[CircuitBreaker] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[CircuitBreaker] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END