DROP TABLE IF EXISTS Movimenti;
DROP TABLE IF EXISTS Prodotti;

CREATE TABLE Prodotti(
	CodiceProdotto NVARCHAR(20) NOT NULL PRIMARY KEY,
	Nome NVARCHAR(100) NOT NULL,
	PrezzoUnitario DECIMAL(10,2) NOT NULL CHECK(PrezzoUnitario>0),
	QuantitaAttuale INT NOT NULL DEFAULT 0 CHECK(QuantitaAttuale>=0),
	SogliaMinima INT NOT NULL DEFAULT 0 CHECK(SogliaMinima>=0)
);

CREATE TABLE Movimenti(
	IdMovimento INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	CodiceProdotto NVARCHAR(20) NOT NULL FOREIGN KEY REFERENCES Prodotti(CodiceProdotto),
	Tipo TINYINT NOT NULL CHECK(Tipo IN (0,1)),--0=cARICO, 1=Scarico
	Quantita INT NOT NULL CHECK(Quantita>0),
	DataMovimento DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE INDEX IX_Movimenti_CodiceProdotto ON Movimenti(CodiceProdotto);