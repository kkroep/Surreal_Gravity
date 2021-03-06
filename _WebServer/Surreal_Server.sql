/*  Database for the game SURREAL GRAVITY
 *  By: Kees Kroep
 */
	
CREATE DATABASE SURREAL;

USE SURREAL;

CREATE TABLE Players(
  Naam VARCHAR(30) NOT NULL,
  Paswoord VARCHAR(30) NOT NULL,
  Gespeeld INT UNSIGNED,
  Gewonnen INT UNSIGNED,
  Wanneer Date,
  PLAYER_Id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY);

CREATE TABLE Participants(
  PLAYER_Id INT UNSIGNED NOT NULL,
  GAME_Id INT UNSIGNED NOT NULL,
  PSRTICIPANTS_Id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY);

CREATE TABLE Games(
  Wanneer Date,
  Server INT UNSIGNED NOT NULL,
  Winnaar INT UNSIGNED,
  Finished BIT,
  Gamemode VARCHAR(10) NOT NULL,
  GAME_Id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY);

ALTER TABLE Participants ADD FOREIGN KEY (GAME_Id) REFERENCES Games(GAME_Id);

ALTER TABLE Participants ADD FOREIGN KEY (PLAYER_Id) REFERENCES Players(PLAYER_Id);


INSERT INTO Players VALUES('Default', 'Default', 45, 20, '2015-1-1', NULL);
INSERT INTO Players VALUES('Kees', 'Kees', 10, 8, '2015-1-1', NULL);
INSERT INTO Players VALUES('Steven', 'Steven', 14, 6, '2015-1-1', NULL);
INSERT INTO Players VALUES('Roberto', 'Roberto', 20, 11, '2015-1-1', NULL);
INSERT INTO Players VALUES('Coen', 'Coen', 17, 9, '2015-1-1', NULL);
INSERT INTO Players VALUES('Kwok', 'Kwok', 14, 8, '2015-1-1', NULL);
INSERT INTO Players VALUES('Ayyoeb', 'Ayyoeb', 9, 4, '2015-1-1', NULL);