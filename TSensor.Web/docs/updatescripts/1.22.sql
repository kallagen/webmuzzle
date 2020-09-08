ALTER TABLE Point ADD CoordinatesChanged bit DEFAULT 0
GO

CREATE TABLE PointCoordinates
(
	PointGuid uniqueidentifier FOREIGN KEY REFERENCES Point(PointGuid) ON DELETE CASCADE,
	ActualDate datetime,
	Longitude decimal(10, 6),
	Latitude decimal(10, 6),
	PRIMARY KEY(PointGuid, ActualDate)
)
GO