ALTER TABLE Point ADD IsMoving bit
GO
ALTER TABLE Point ADD LastMovingDate datetime 
GO

DROP TABLE PointCoordinates
GO

CREATE TABLE PointCoordinates (
    PointGuid UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Point(PointGuid) ON DELETE CASCADE NOT NULL,
    ActualDate DATETIME DEFAULT (getdate()) NOT NULL,
    Longitude DECIMAL(10, 6) NOT NULL,
    Latitude DECIMAL(10, 6) NOT NULL
)
GO