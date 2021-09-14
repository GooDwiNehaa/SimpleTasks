CREATE TABLE Users(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);
CREATE TABLE Tasks(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Subject TEXT NOT NULL,
    Description TEXT NULL
);
CREATE TABLE TasksUsers(
	TaskId INTEGER,
	AssigneeId INTEGER,
	FOREIGN KEY(TaskId) REFERENCES Tasks(Id),
	FOREIGN KEY(AssigneeId) REFERENCES Users(Id),
	PRIMARY KEY(TaskId, AssigneeId)
);
INSERT INTO Users(Name) VALUES('User0'),
							  ('User1'),
							  ('User2'),
							  ('User3'),
							  ('User4');