CREATE TABLE users(
	id serial PRIMARY KEY,
	name varchar(40) NOT NULL,
	sex varchar(1) NOT NULL,
	weight integer NOT NULL
);

INSERT INTO users(id, name, sex, weight)
VALUES(1, 'Andrew', 'M', 80);
INSERT INTO users(id, name, sex, weight)
VALUES(2, 'Ann', 'F', 50);
INSERT INTO users(id, name, sex, weight)
VALUES(3, 'Sally', 'F', 45);
INSERT INTO users(id, name, sex, weight)
VALUES(4, 'John', 'M', 75);

SELECT * FROM users;