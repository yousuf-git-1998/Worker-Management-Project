CREATE TABLE projects
(
	projectid INT IDENTITY PRIMARY KEY,
	projectname NVARCHAR(40) NOT NULL,
	startdate DATE NOT NULL,
	iscompleted BIT
)
GO
CREATE TABLE workers
(
	workerid INT IDENTITY PRIMARY KEY,
	workername NVARCHAR(40) NOT NULL,
	phone NVARCHAR(20) NOT NULL,
	payrate MONEY NOT NULL,
	skill NVARCHAR(30) NOT NULL,
	picture NVARCHAR(50) NOT NULL,
	projectid INT REFERENCES projects(projectid)
)
GO
CREATE TABLE commonskills
(
	skillid INT IDENTITY PRIMARY KEY,
	skillname NVARCHAR(40) NOT NULL
)
GO