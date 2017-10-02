CREATE TABLE [dbo].[SwitchingDeviceNormalState]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [State] NVARCHAR(20) NOT NULL , 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_SwitchingDeviceNormalState_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_SwitchingDeviceNormalState_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_SwitchingDeviceNormalState_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_SwitchingDeviceNormalState_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [CK_SwitchingDeviceNormalState_State] CHECK (State = 'NormallyClosed' OR State = 'NormallyOpen')
)

GO

CREATE INDEX [IX_SwitchingDeviceNormalState_State] ON [dbo].[SwitchingDeviceNormalState] ([State])

GO

CREATE TRIGGER [dbo].[Trigger_SwitchingDeviceNormalStateUpdated]
    ON [dbo].[SwitchingDeviceNormalState]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[SwitchingDeviceNormalState] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[SwitchingDeviceNormalState] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END