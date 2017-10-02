CREATE TABLE [dbo].[CurrentInjectionPhasorGroup]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Acronym] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [MeasuredConnectedNode] UNIQUEIDENTIFIER NULL, 
    [Enabled] BIT NOT NULL DEFAULT 1, 
    [UseStatusFlag] BIT NOT NULL DEFAULT 1, 
    [StatusWord] UNIQUEIDENTIFIER NULL, 
    [PositiveSequence] UNIQUEIDENTIFIER NULL, 
    [PhaseA] UNIQUEIDENTIFIER NULL, 
    [PhaseB] UNIQUEIDENTIFIER NULL, 
    [PhaseC] UNIQUEIDENTIFIER NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_CurrentInjectionPhasorGroup_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_CurrentInjectionPhasorGroup_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_CurrentInjectionPhasorGroup_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_CurrentInjectionPhasorGroup_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_CurrentInjectionPhasorGroup_Node] FOREIGN KEY ([MeasuredConnectedNode]) REFERENCES [Node]([Uid]) ON DELETE CASCADE ON UPDATE CASCADE, 
    CONSTRAINT [FK_CurrentInjectionPhasorGroup_StatusWord] FOREIGN KEY ([StatusWord]) REFERENCES [StatusWord]([Uid]), 
    CONSTRAINT [FK_CurrentInjectionPhasorGroup_PositiveSequence] FOREIGN KEY ([PositiveSequence]) REFERENCES [Phasor]([Uid]), 
    CONSTRAINT [FK_CurrentInjectionPhasorGroup_PhaseA] FOREIGN KEY ([PhaseA]) REFERENCES [Phasor]([Uid]), 
    CONSTRAINT [FK_CurrentInjectionPhasorGroup_PhaseB] FOREIGN KEY ([PhaseB]) REFERENCES [Phasor]([Uid]), 
    CONSTRAINT [FK_CurrentInjectionPhasorGroup_PhaseC] FOREIGN KEY ([PhaseC]) REFERENCES [Phasor]([Uid]), 
    CONSTRAINT [CK_CurrentInjectionPhasorGroup_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_CurrentInjectionPhasorGroup_Number] CHECK (Number > 0), 
    CONSTRAINT [CK_CurrentInjectionPhasorGroup_Acronym] CHECK (Acronym NOT LIKE '%[^A-Z]%')
)

GO

CREATE INDEX [IX_CurrentInjectionPhasorGroup_Id] ON [dbo].[CurrentInjectionPhasorGroup] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_CurrentInjectionPhasorGroupUpdate]
    ON [dbo].[CurrentInjectionPhasorGroup]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[CurrentInjectionPhasorGroup] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[CurrentInjectionPhasorGroup] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END