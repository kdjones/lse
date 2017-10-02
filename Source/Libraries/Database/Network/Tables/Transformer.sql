CREATE TABLE [dbo].[Transformer]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [ParentSubstation] UNIQUEIDENTIFIER NOT NULL, 
    [FromNode] UNIQUEIDENTIFIER NOT NULL, 
    [ToNode] UNIQUEIDENTIFIER NOT NULL, 
    [FromNodeConnectionType] UNIQUEIDENTIFIER NOT NULL, 
    [ToNodeConnectionType] UNIQUEIDENTIFIER NOT NULL, 
    [FromNodeCurrent] UNIQUEIDENTIFIER NOT NULL, 
    [ToNodeCurrent] UNIQUEIDENTIFIER NOT NULL, 
    [Tap] UNIQUEIDENTIFIER NOT NULL, 
    [TapInputMeasurementKey] NVARCHAR(200) NOT NULL DEFAULT 'Undefined', 
    [TapOutputKey] NVARCHAR(200) NOT NULL DEFAULT 'Undefined', 
    [FixedTapPosition] INT NOT NULL DEFAULT 0, 
    [EnableUltc] BIT NOT NULL DEFAULT 0, 
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
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_Transformer_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Transformer_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_Transformer_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Transformer_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_Transformer_Substation] FOREIGN KEY ([ParentSubstation]) REFERENCES [Substation]([Uid]) ON DELETE CASCADE ON UPDATE CASCADE, 
    CONSTRAINT [FK_Transformer_FromNode] FOREIGN KEY ([FromNode]) REFERENCES [Node]([Uid]), 
    CONSTRAINT [FK_Transformer_ToNode] FOREIGN KEY ([ToNode]) REFERENCES [Node]([Uid]), 
    CONSTRAINT [FK_Transformer_FromNodeConnectionType] FOREIGN KEY ([FromNodeConnectionType]) REFERENCES [TransformerConnectionType]([Uid]), 
    CONSTRAINT [FK_Transformer_ToNodeConnectionType] FOREIGN KEY ([ToNodeConnectionType]) REFERENCES [TransformerConnectionType]([Uid]), 
    CONSTRAINT [FK_Transformer_FromNodeCurrent] FOREIGN KEY ([FromNodeCurrent]) REFERENCES [CurrentFlowPhasorGroup]([Uid]), 
    CONSTRAINT [FK_Transformer_ToNodeCurrent] FOREIGN KEY ([ToNodeCurrent]) REFERENCES [CurrentFlowPhasorGroup]([Uid]), 
    CONSTRAINT [FK_Transformer_Tap] FOREIGN KEY ([Tap]) REFERENCES [TapConfiguration]([Uid]) ON UPDATE CASCADE,
	CONSTRAINT [CK_Transformer_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_Transformer_Number] CHECK (Number > 0), 
)

GO

CREATE INDEX [IX_Transformer_Id] ON [dbo].[Transformer] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_TransformerUpdated]
    ON [dbo].[Transformer]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[Transformer] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[Transformer] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END