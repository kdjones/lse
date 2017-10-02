CREATE TABLE [dbo].[Node]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Acronym] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL, 
    [ParentSubstation] UNIQUEIDENTIFIER NULL, 
    [ParentTransmissionLine] UNIQUEIDENTIFIER NULL, 
    [BaseKv] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_Node_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Node_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_Node_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Node_LastEditedBy Default(SUSER_NAME()), 
    CONSTRAINT [FK_Node_Substation] FOREIGN KEY ([ParentSubstation]) REFERENCES [Substation]([Uid]), 
    CONSTRAINT [FK_Node_TransmissionLine] FOREIGN KEY ([ParentTransmissionLine]) REFERENCES [TransmissionLine]([Uid]), 
    CONSTRAINT [FK_Node_VoltageLevel] FOREIGN KEY ([BaseKv]) REFERENCES [VoltageLevel]([Uid]), 
    CONSTRAINT [CK_Node_Id] CHECK (Id > 0), 
    CONSTRAINT [CK_Node_Number] CHECK (Number > 0), 
    CONSTRAINT [CK_Node_Acronym] CHECK (Acronym NOT LIKE '%[^A-Z]%')
)

GO

CREATE INDEX [IX_Node_Id] ON [dbo].[Node] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_NodeUpdated]
    ON [dbo].[Node]
    FOR DELETE, INSERT, UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[Node] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[Node] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END