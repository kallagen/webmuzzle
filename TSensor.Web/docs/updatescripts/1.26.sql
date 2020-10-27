CREATE TABLE PointType
(
	PointTypeGuid uniqueidentifier PRIMARY KEY DEFAULT NEWID(),
	Name nvarchar(128) NOT NULL,
	[Image] nvarchar(MAX) DEFAULT NULL
)
GO

ALTER TABLE [Point] ADD PointTypeGuid uniqueidentifier FOREIGN KEY REFERENCES PointType(PointTypeGuid) ON DELETE SET NULL
GO
