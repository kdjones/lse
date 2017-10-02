USE SynchrophasorAnalytics

PRINT 'Inserting default values into BreakerStatusBit table'

INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV64', GETUTCDATE(), 'Walter White', GETUTCDATE(), 'Walter White');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV63', GETUTCDATE(), 'Santa Claus', GETUTCDATE(), 'Santa Claus');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV62', GETUTCDATE(), 'James T. Kirk', GETUTCDATE(), 'James T. Kirk');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV61', GETUTCDATE(), 'Luke Skywalker', GETUTCDATE(), 'Luke Skywalker');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV60', GETUTCDATE(), 'Bruce Wayne', GETUTCDATE(), 'Bruce Wayne');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV59', GETUTCDATE(), 'Frank Underwood', GETUTCDATE(), 'Frank Underwood');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV58', GETUTCDATE(), 'Tyrion Lannister', GETUTCDATE(), 'Tyrion Lannister');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV57', GETUTCDATE(), 'Neil DeGrasse Tyson', GETUTCDATE(), 'Neil DeGrasse Tyson');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV56', GETUTCDATE(), 'Peter Parker', GETUTCDATE(), 'Peter Parker');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV55', GETUTCDATE(), 'Isaac Asimov', GETUTCDATE(), 'Isaac Asimov');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV54', GETUTCDATE(), 'Joey Tribbiani', GETUTCDATE(), 'Joey Tribbiani');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV53', GETUTCDATE(), 'Jerry Sienfield', GETUTCDATE(), 'Jerry Sienfield');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV52', GETUTCDATE(), 'Edmond Dantes', GETUTCDATE(), 'Edmond Dantes');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV51', GETUTCDATE(), 'Clark Kent', GETUTCDATE(), 'Clark Kent');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV50', GETUTCDATE(), 'Tony Stark', GETUTCDATE(), 'Tony Stark');
INSERT INTO BreakerStatusBit (Uid, Bit, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'PSV49', GETUTCDATE(), 'Bruce Banner', GETUTCDATE(), 'Bruce Banner');

PRINT 'Inserting default values into ShuntImpedanceCalculationMethod table'

INSERT INTO ShuntImpedanceCalculationMethod (Uid, Method, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'UseModeledImpedance', GETUTCDATE(), 'Fox Mulder', GETUTCDATE(), 'Fox Mulder');
INSERT INTO ShuntImpedanceCalculationMethod (Uid, Method, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'CalculateFromRating', GETUTCDATE(), 'Dana Scully', GETUTCDATE(), 'Dana Scully');

PRINT 'Inserting default values into SwitchingDeviceNormalState table'

INSERT INTO SwitchingDeviceNormalState (Uid, State, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'NormallyClosed', GETUTCDATE(), 'Jason Bourne', GETUTCDATE(), 'Jason Bourne');
INSERT INTO SwitchingDeviceNormalState (Uid, State, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'NormallyOpen', GETUTCDATE(), 'Morpheus', GETUTCDATE(), 'Morpheus');

PRINT 'Inserting default values into TransformerConnectionType table'

INSERT INTO TransformerConnectionType (Uid, ConnectionType, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'Delta', GETUTCDATE(), 'Robocop', GETUTCDATE(), 'Robocop');
INSERT INTO TransformerConnectionType (Uid, ConnectionType, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES (NEWID(), 'Wye', GETUTCDATE(), 'Terminator', GETUTCDATE(), 'Terminator');

PRINT 'Inserting default values into PhaseSelection table'

INSERT INTO PhaseSelection (Uid, Configuration, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES(NEWID(), 'PositiveSequence', GETUTCDATE(), 'James Bond', GETUTCDATE(), 'James Bond');
INSERT INTO PhaseSelection (Uid, Configuration, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES(NEWID(), 'ThreePhase', GETUTCDATE(), 'Liam Neeson', GETUTCDATE(), 'Liam Neeson');

PRINT 'Inserting default values into CurrentFlowPostProcessingSetting table'

INSERT INTO CurrentFlowPostProcessingSetting (Uid, Setting, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES(NEWID(), 'ProcessOnlyMeasuredBranches', GETUTCDATE(), 'Buzz Lightyear', GETUTCDATE(), 'Buzz Lightyear');
INSERT INTO CurrentFlowPostProcessingSetting (Uid, Setting, CreatedOn, CreatedBy, LastEditedOn, LastEditedBy) VALUES(NEWID(), 'ProcessBranchesByNodeObservability', GETUTCDATE(), 'Dexter Morgan', GETUTCDATE(), 'Dexter Morgan');

