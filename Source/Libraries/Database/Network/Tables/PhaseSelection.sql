CREATE TABLE [dbo].[PhaseSelection]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Configuration] NVARCHAR(20) NULL, 
    [CreatedOn] TIME NULL, 
    [CreatedBy] NVARCHAR(200) NULL, 
    [LastEditedOn] TIME NULL, 
    [LastEditedBy] NVARCHAR(200) NULL, 
    CONSTRAINT [CK_PhaseSelection_Configuration] CHECK (Configuration = 'PositiveSequence' OR Configuration = 'ThreePhase')
)

GO

CREATE INDEX [IX_PhaseSelection_Configuration] ON [dbo].[PhaseSelection] ([Configuration])

GO

CREATE TRIGGER [dbo].[Trigger_PhaseSelectionUpdated]
    ON [dbo].[PhaseSelection]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[PhaseSelection] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[PhaseSelection] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END