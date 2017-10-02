CREATE TABLE [dbo].[TransformerConnectionType]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ConnectionType] NCHAR(10) NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_TransformerConnectionType_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_TransformerConnectionType_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_TransformerConnectionType_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_TransformerConnectionType_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [CK_TransformerConnectionType_ConnectionType] CHECK (ConnectionType = 'Delta' OR ConnectionType = 'Wye')
)

GO

CREATE INDEX [IX_TransformerConnectionType_ConnectionType] ON [dbo].[TransformerConnectionType] ([ConnectionType])

GO

CREATE TRIGGER [dbo].[Trigger_TransformerConnectionTypeUpdated]
    ON [dbo].[TransformerConnectionType]
    FOR DELETE, INSERT, UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[TransformerConnectionType] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[TransformerConnectionType] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END