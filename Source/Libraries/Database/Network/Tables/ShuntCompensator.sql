CREATE TABLE [dbo].[ShuntCompensator]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [ParentSubstation] UNIQUEIDENTIFIER NULL, 
    [ConnectedNode] UNIQUEIDENTIFIER NULL, 
    [CurrentInjection] UNIQUEIDENTIFIER NULL, 
    [ImpedanceCalculationMethod] UNIQUEIDENTIFIER NOT NULL, 
    [NominalMvar] FLOAT NOT NULL DEFAULT 100, 
    [R1] FLOAT NOT NULL DEFAULT 0.0, 
    [R2] FLOAT NOT NULL DEFAULT 0.0, 
    [R3] FLOAT NOT NULL DEFAULT 0.0, 
    [R4] FLOAT NOT NULL DEFAULT 0.0, 
    [R5] FLOAT NOT NULL DEFAULT 0.0, 
    [R6] FLOAT NOT NULL DEFAULT 0.0, 
    [X1] FLOAT NOT NULL DEFAULT 0.01, 
    [X2] FLOAT NOT NULL DEFAULT 0.0, 
    [X3] FLOAT NOT NULL DEFAULT 0.01, 
    [X4] FLOAT NOT NULL DEFAULT 0.0, 
    [X5] FLOAT NOT NULL DEFAULT 0.0, 
    [X6] FLOAT NOT NULL DEFAULT 0.01, 
    [G1] FLOAT NOT NULL DEFAULT 0.0, 
    [G2] FLOAT NOT NULL DEFAULT 0.0, 
    [G3] FLOAT NOT NULL DEFAULT 0.0, 
    [G4] FLOAT NOT NULL DEFAULT 0.0, 
    [G5] FLOAT NOT NULL DEFAULT 0.0, 
    [G6] FLOAT NOT NULL DEFAULT 0.0, 
    [B1] FLOAT NOT NULL DEFAULT 0.0, 
    [B2] FLOAT NOT NULL DEFAULT 0.0, 
    [B3] FLOAT NOT NULL DEFAULT 0.0, 
    [B4] FLOAT NOT NULL DEFAULT 0.0, 
    [B5] FLOAT NOT NULL DEFAULT 0.0, 
    [B6] FLOAT NOT NULL DEFAULT 0.0, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_ShuntCompensator_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_ShuntCompensator_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_ShuntCompensator_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_ShuntCompensator_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_ShuntCompensator_Substation] FOREIGN KEY ([ParentSubstation]) REFERENCES [Substation]([Uid]) ON DELETE CASCADE ON UPDATE CASCADE, 
    CONSTRAINT [FK_ShuntCompensator_Node] FOREIGN KEY ([ConnectedNode]) REFERENCES [Node]([Uid]), 
    CONSTRAINT [FK_ShuntCompensator_CurrentInjection] FOREIGN KEY ([CurrentInjection]) REFERENCES [CurrentInjectionPhasorGroup]([Uid]), 
    CONSTRAINT [FK_ShuntCompensator_ImpedanceCalculationMethod] FOREIGN KEY ([ImpedanceCalculationMethod]) REFERENCES [ShuntImpedanceCalculationMethod]([Uid]) ON UPDATE CASCADE,
	CONSTRAINT [CK_ShuntCompensator_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_ShuntCompensator_Number] CHECK (Number > 0)
)

GO

CREATE INDEX [IX_ShuntCompensator_Id] ON [dbo].[ShuntCompensator] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_ShuntCompensatorUpdated]
    ON [dbo].[ShuntCompensator]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[ShuntCompensator] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[ShuntCompensator] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END