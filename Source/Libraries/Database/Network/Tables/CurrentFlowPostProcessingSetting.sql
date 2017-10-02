CREATE TABLE [dbo].[CurrentFlowPostProcessingSetting]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Setting] NVARCHAR(50) NOT NULL, 
    [CreatedOn] TIME NOT NULL, 
    [CreatedBy] NVARCHAR(200) NOT NULL, 
    [LastEditedOn] TIME NOT NULL, 
    [LastEditedBy] NVARCHAR(200) NOT NULL, 
    CONSTRAINT [CK_CurrentFlowPostProcessingSetting_Setting] CHECK (Setting = 'ProcessOnlyMeasuredBranches' OR Setting = 'ProcessBranchesByNodeObservability')
)

GO

CREATE INDEX [IX_CurrentFlowPostProcessingSetting_Setting] ON [dbo].[CurrentFlowPostProcessingSetting] ([Setting])

GO

CREATE TRIGGER [dbo].[Trigger_CurrentFlowPostProcessingSettingUpdated]
    ON [dbo].[CurrentFlowPostProcessingSetting]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[CurrentFlowPostProcessingSetting] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[CurrentFlowPostProcessingSetting] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END