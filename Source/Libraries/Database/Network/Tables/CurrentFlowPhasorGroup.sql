CREATE TABLE [dbo].[CurrentFlowPhasorGroup]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Acronym] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [MeasuredFromNode] UNIQUEIDENTIFIER NULL, 
    [MeasuredToNode] UNIQUEIDENTIFIER NULL, 
    [Enabled] BIT NOT NULL DEFAULT 1, 
    [UseStatusFlag] BIT NOT NULL DEFAULT 1, 
    [StatusWord] UNIQUEIDENTIFIER NULL, 
    [PositiveSequence] UNIQUEIDENTIFIER NULL, 
    [PhaseA] UNIQUEIDENTIFIER NULL, 
    [PhaseB] UNIQUEIDENTIFIER NULL, 
    [PhaseC] UNIQUEIDENTIFIER NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_CurrentFlowPhasorGroup_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_CurrentFlowPhasorGroup_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_CurrentFlowPhasorGroup_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_CurrentFlowPhasorGroup_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_CurrentFlowPhasorGroup_FromNode] FOREIGN KEY ([MeasuredFromNode]) REFERENCES [Node]([Uid]), 
    CONSTRAINT [FK_CurrentFlowPhasorGroup_ToNode] FOREIGN KEY ([MeasuredToNode]) REFERENCES [Node]([Uid]), 
    CONSTRAINT [FK_CurrentFlowPhasorGroup_StatusWord] FOREIGN KEY ([StatusWord]) REFERENCES [StatusWord]([Uid]), 
    CONSTRAINT [FK_CurrentFlowPhasorGroup_PositiveSequence] FOREIGN KEY ([PositiveSequence]) REFERENCES [Phasor]([Uid]), 
    CONSTRAINT [FK_CurrentFlowPhasorGroup_PhaseA] FOREIGN KEY ([PhaseA]) REFERENCES [Phasor]([Uid]), 
    CONSTRAINT [FK_CurrentFlowPhasorGroup_PhaseB] FOREIGN KEY ([PhaseB]) REFERENCES [Phasor]([Uid]), 
    CONSTRAINT [FK_CurrentFlowPhasorGroup_PhaseC] FOREIGN KEY ([PhaseC]) REFERENCES [Phasor]([Uid]),    
	CONSTRAINT [CK_CurrentFlowPhasorGroup_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_CurrentFlowPhasorGroup_Number] CHECK (Number > 0), 
    CONSTRAINT [CK_CurrentFlowPhasorGroup_Acronym] CHECK (Acronym NOT LIKE '%[^A-Z]%')
)

GO

CREATE TRIGGER [dbo].[Trigger_CurrentFlowPhasorGroupUpdate]
    ON [dbo].[CurrentFlowPhasorGroup]
    FOR DELETE, INSERT, UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[CurrentFlowPhasorGroup] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[CurrentFlowPhasorGroup] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END
GO

CREATE INDEX [IX_CurrentFlowPhasorGroup_Id] ON [dbo].[CurrentFlowPhasorGroup] ([Id])
