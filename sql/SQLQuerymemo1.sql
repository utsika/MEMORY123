DELETE FROM Card;

SELECT * FROM Card;

ALTER TABLE GameCard
ADD [IsFlipped] bit not null;

ALTER TABLE GameCard
DROP COLUMN [X], [Y];

SELECT * FROM GameCard;

SELECT * FROM CardLibrary;

DELETE FROM CardLibrary;

INSERT INTO CardLibrary (CardName) VALUES
('katt'),
('hund'),
('fisk'),
('elefant'),
('leopard'),
('varg'),
('gris'),
('kanin');

ALTER TABLE GameCard
ADD [CardID] int not null;

ALTER TABLE Game_Card
DROP CONSTRAINT [FK_Game_Card_CardID];

ALTER TABLE Game
DROP CONSTRAINT [DEFAULT_Game_State];

SELECT * FROM Game;

ALTER TABLE Game
DROP COLUMN [State];

DELETE FROM Game;

ALTER TABLE Game
ADD [Player1] int not null,
	[Player2] int null,
	[State] varchar(20) not null,
	[CurrentPlayer] int not null,
	[RoomCode] varchar(10) not null,
	[Winner] int null,
	[AmountOfPairs] int not null;


DELETE FROM Game;