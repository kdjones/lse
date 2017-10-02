CREATE TABLE [dbo].[VoltagePhasorGroup]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Acronym] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [MeasuredNode] UNIQUEIDENTIFIER NULL, 
    [Enabled] BIT NOT NULL DEFAULT 1, 
    [UseStatusFlag] BIT NOT NULL DEFAULT 1, 
    [StatusWord] UNIQUEIDENTIFIER NULL, 
    [PositiveSequence] UNIQUEIDENTIFIER NULL, 
    [PhaseA] UNIQUEIDENTIFIER NULL, 
    [PhaseB] UNIQUEIDENTIFIER NULL, 
    [PhaseC] UNIQUEIDENTIFIER NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_VoltagePhasorGroup_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_VoltagePhasorGroup_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_VoltagePhasorGroup_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_VoltagePhasorGroup_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_VoltagePhasorGroup_StatusWord] FOREIGN KEY ([StatusWord]) REFERENCES [StatusWord]([Uid]),
    CONSTRAINT [FK_VoltagePhasorGroup_PositiveSequencePhasor] FOREIGN KEY ([PositiveSequence]) REFERENCES [Phasor]([Uid]), 
    CONSTRAINT [FK_VoltagePhasorGroup_PhaseAPhasor] FOREIGN KEY ([PhaseA]) REFERENCES [Phasor]([Uid]), 
    CONSTRAINT [FK_VoltagePhasorGroup_PhaseBPhasor] FOREIGN KEY ([PhaseB]) REFERENCES [Phasor]([Uid]), 
    CONSTRAINT [FK_VoltagePhasorGroup_PhaseCPhasor] FOREIGN KEY ([PhaseC]) REFERENCES [Phasor]([Uid]), 
    CONSTRAINT [FK_VoltagePhasorGroup_Node] FOREIGN KEY ([MeasuredNode]) REFERENCES [Node]([Uid]),
	CONSTRAINT [CK_VoltagePhasorGroup_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_VoltagePhasorGroup_Number] CHECK (Number > 0), 
    CONSTRAINT [CK_VoltagePhasorGroup_Acronym] CHECK (Acronym NOT LIKE '%[^A-Z]%')
)

GO

CREATE INDEX [IX_VoltagePhasorGroup_Id] ON [dbo].[VoltagePhasorGroup] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_VoltagePhasorGroupUpdated]
    ON [dbo].[VoltagePhasorGroup]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[VoltagePhasorGroup] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[VoltagePhasorGroup] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END