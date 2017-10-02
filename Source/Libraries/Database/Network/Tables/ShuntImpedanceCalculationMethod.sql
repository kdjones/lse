CREATE TABLE [dbo].[ShuntImpedanceCalculationMethod]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Method] NVARCHAR(20) NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_ShuntImpedanceCalculationMethod_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_ShuntImpedanceCalculationMethod_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_ShuntImpedanceCalculationMethod_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_ShuntImpedanceCalculationMethod_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [CK_ShuntImpedanceCalculationMethod_Method] CHECK (Method = 'UseModeledImpedance' OR Method = 'CalculateFromRating')
)

GO

CREATE INDEX [IX_ShuntImpedanceCalculationMethod_Method] ON [dbo].[ShuntImpedanceCalculationMethod] ([Method])

GO

CREATE TRIGGER [dbo].[Trigger_ShuntImpedanceCalculationMethodUpdated]
    ON [dbo].[ShuntImpedanceCalculationMethod]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[ShuntImpedanceCalculationMethod] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[ShuntImpedanceCalculationMethod] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END