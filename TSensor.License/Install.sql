CREATE TABLE License
(
	LicenseGuid UNIQUEIDENTIFIER PRIMARY KEY,
	Data nvarchar(MAX),
	Name nvarchar(128),
	ExpireDate date NOT NULL,
	SensorCount int NOT NULL,
	CreationDateUTC datetime NOT NULL DEFAULT GETUTCDATE(),
	IsActivated bit NOT NULL DEFAULT 0
)
GO

CREATE TABLE LicenseActivation
(
	LicenseGuid uniqueidentifier FOREIGN KEY REFERENCES License(LicenseGuid) ON DELETE NO ACTION,
	ActivationDateUTC datetime NOT NULL DEFAULT GETUTCDATE(),
	ActivationIp nvarchar(16)
)
GO