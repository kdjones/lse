CREATE TABLE [dbo].[TransmissionLine]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Acronym] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [ParentDivision] UNIQUEIDENTIFIER NOT NULL, 
    [FromSubstation] UNIQUEIDENTIFIER NOT NULL, 
    [ToSubstation] UNIQUEIDENTIFIER NOT NULL, 
    [FromNode] UNIQUEIDENTIFIER NOT NULL, 
    [ToNode] UNIQUEIDENTIFIER NOT NULL, 
    [FromSubstationCurrent] UNIQUEIDENTIFIER NOT NULL, 
    [ToSubstationCurrent] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_TransmissionLine_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_TransmissionLine_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_TransmissionLine_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_TransmissionLine_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [FK_TransmissionLine_Division] FOREIGN KEY ([ParentDivision]) REFERENCES [Division]([Uid]), 
    CONSTRAINT [FK_TransmissionLine_FromSubstation] FOREIGN KEY ([FromSubstation]) REFERENCES [Substation]([Uid]), 
    CONSTRAINT [FK_TransmissionLine_ToSubstation] FOREIGN KEY ([ToSubstation]) REFERENCES [Substation]([Uid]), 
    CONSTRAINT [FK_TransmissionLine_FromNode] FOREIGN KEY ([FromNode]) REFERENCES [Node]([Uid]), 
    CONSTRAINT [FK_TransmissionLine_ToNode] FOREIGN KEY ([ToNode]) REFERENCES [Node]([Uid]), 
    CONSTRAINT [FK_TransmissionLine_FromSubstationCurrent] FOREIGN KEY ([FromSubstationCurrent]) REFERENCES [CurrentFlowPhasorGroup]([Uid]), 
    CONSTRAINT [FK_TransmissionLine_ToSubstationCurrent] FOREIGN KEY ([ToSubstationCurrent]) REFERENCES [CurrentFlowPhasorGroup]([Uid]),	
	CONSTRAINT [CK_TransmissionLine_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_TransmissionLine_Number] CHECK (Number > 0), 
    CONSTRAINT [CK_TransmissionLine_Acronym] CHECK (Acronym NOT LIKE '%[^A-Z]%')
)

GO

CREATE INDEX [IX_TransmissionLine_Id] ON [dbo].[TransmissionLine] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_TransmissionLineUpdated]
    ON [dbo].[TransmissionLine]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[TransmissionLine] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[TransmissionLine] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END