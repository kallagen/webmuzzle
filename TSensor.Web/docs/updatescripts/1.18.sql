ALTER TABLE Product Add IsGas bit NOT NULL DEFAULT 0
GO

UPDATE ActualSensorValue SET pressureMeasuring = 0
GO
ALTER TABLE ActualSensorValue ALTER COLUMN pressureMeasuring DECIMAL(10, 2) NULL
GO

UPDATE SensorValue SET pressureMeasuring = 0
GO
ALTER TABLE SensorValue ALTER COLUMN pressureMeasuring DECIMAL(10, 2) NULL
GO

ALTER TABLE Point ADD Longitude decimal(10, 6)
GO
ALTER TABLE Point ADD Latitude decimal(10, 6)
GO