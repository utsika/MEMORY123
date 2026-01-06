CREATE TABLE [CardLibrary] (
	[CardID] INT IDENTITY (1, 1) NOT NULL,
	[CardName] NVARCHAR (100) NOT NULL
);

SELECT * FROM [CardLibrary];

INSERT INTO [CardLibrary] ([CardName]) VALUES 
('Cat'),
('Dog'),
('Cat');


SELECT * FROM [Card];
--TÖMMER CARD--
DELETE FROM [Card];