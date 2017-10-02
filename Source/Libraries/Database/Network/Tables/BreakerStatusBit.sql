CREATE TABLE [dbo].[BreakerStatusBit]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Bit] NVARCHAR(10) NOT NULL, 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_BreakerStatusBit_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_BreakerStatusBit_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_BreakerStatusBit_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_BreakerStatusBit_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [CK_BreakerStatusBit_Bit] CHECK (Bit = 'PSV64' OR Bit = 'PSV63' OR Bit = 'PSV62' OR Bit = 'PSV61' OR Bit = 'PSV60' OR Bit = 'PSV59' OR Bit = 'PSV58' OR Bit = 'PSV57' OR Bit = 'PSV56' OR Bit = 'PSV55' OR Bit = 'PSV54' OR Bit = 'PSV53' OR Bit = 'PSV52' OR Bit = 'PSV51' OR Bit = 'PSV50' OR Bit = 'PSV49')
)

GO

CREATE INDEX [IX_BreakerStatusBit_Bit] ON [dbo].[BreakerStatusBit] ([Bit])

GO

CREATE TRIGGER [dbo].[Trigger_BreakerStatusBitUpdate]
    ON [dbo].[BreakerStatusBit]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[BreakerStatusBit] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[BreakerStatusBit] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END