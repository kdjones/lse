CREATE TABLE [dbo].[Phasor]
(
	[Uid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [MeasurementMagnitudeKey] NVARCHAR(200) NOT NULL DEFAULT 'Undefined', 
    [MeasurementAngleKey] NVARCHAR(50) NOT NULL DEFAULT 'Undefined', 
    [MeasurementVariance] FLOAT NOT NULL DEFAULT 0.002, 
    [RatioCorrectionFactor] FLOAT NOT NULL DEFAULT 1.0, 
    [PhaseAngleCorrectionFactor] FLOAT NOT NULL DEFAULT 0.0, 
    [ShouldBeCalibrated] BIT NOT NULL DEFAULT 0, 
    [EstimateMagnitudeKey] NVARCHAR(200) NOT NULL DEFAULT 'Undefined', 
    [EstimateAngleKey] NVARCHAR(200) NOT NULL DEFAULT 'Undefined', 
    [MagnitudeResidualKey] NVARCHAR(200) NOT NULL DEFAULT 'Undefined', 
    [AngleResidualKey] NVARCHAR(200) NOT NULL DEFAULT 'Undefined', 
    [CreatedOn] TIME NOT NULL CONSTRAINT DF_Phasor_CreatedOn DEFAULT(GETUTCDATE()), 
    [CreatedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Phasor_CreatedBy DEFAULT(SUSER_NAME()), 
    [LastEditedOn] TIME NOT NULL CONSTRAINT DF_Phasor_LastEditedOn DEFAULT(GETUTCDATE()), 
    [LastEditedBy] NVARCHAR(200) NOT NULL CONSTRAINT DF_Phasor_LastEditedBy DEFAULT(SUSER_NAME())
)

GO


CREATE TRIGGER [dbo].[Trigger_PhasorUpdated]
    ON [dbo].[Phasor]
    FOR DELETE, INSERT, UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[Phasor] SET LastEditedOn=GETUTCDATE() WHERE Uid in (SELECT Uid FROM INSERTED)
		UPDATE [dbo].[Phasor] SET LastEditedBy=SUSER_NAME() WHERE Uid in (SELECT Uid FROM INSERTED)
    END