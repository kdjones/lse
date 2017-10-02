CREATE TABLE [dbo].[Model]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Id] INT NOT NULL, 
    [Number] INT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Acronym] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(200) NULL,
	[PhaseSelection] UNIQUEIDENTIFIER NULL, 
	[CurrentFlowPostProcessingSetting] UNIQUEIDENTIFIER NULL,
	[AcceptsMeasurements] BIT NOT NULL DEFAULT 1,
	[AcceptsEstimates] BIT NOT NULL DEFAULT 0,
    [ReturnsStateEstimate] BIT NOT NULL DEFAULT 1,
    [ReturnsCurrentFlow] BIT NOT NULL DEFAULT 0,
    [ReturnsVoltageResiduals] BIT NOT NULL DEFAULT 0,
    [ReturnsCurrentResiduals] BIT NOT NULL DEFAULT 0,
    [ReturnsCircuitBreakerStatus] BIT NOT NULL DEFAULT 0,
    [ReturnsSwitchStatus] BIT NOT NULL DEFAULT 0,
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_Model_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Model_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_Model_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Model_LastEditedBy DEFAULT(SUSER_NAME()), 
    CONSTRAINT [CK_Model_Id] CHECK (Id > 0), 
	CONSTRAINT [CK_Model_Number] CHECK (Number > 0),
    CONSTRAINT [CK_Model_Acronym] CHECK (Acronym NOT LIKE '%[^A-Z]%')
)

GO

CREATE INDEX [IX_Model_Id] ON [dbo].[Model] ([Id])

GO

CREATE TRIGGER [dbo].[Trigger_ModelUpdated]
    ON [dbo].[Model]
    FOR UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[Model] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[Model] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END
GO
