CREATE TABLE IF NOT EXISTS Users
(   
id INT GENERATED ALWAYS AS IDENTITY,
Username VARCHAR(50) NOT NULL,
password varchar NOT NULL,
email VARCHAR (100),
salt bytea NOT NULL,
PRIMARY KEY(id)
);

CREATE TABLE IF NOT EXISTS Users_Roles
(   
id INT GENERATED ALWAYS AS IDENTITY,
user_Id INT NOT NULL,
user_right VARCHAR (100) NOT NULL,
PRIMARY KEY(id),
      CONSTRAINT fk_userRoles
      FOREIGN KEY(user_Id) 
	  REFERENCES Users(id)
);

CREATE TABLE reservations(
   id INT GENERATED ALWAYS AS IDENTITY,
   desk_id INT,
   userName VARCHAR(50),
    date_creation timestamp not null default CURRENT_TIMESTAMP, 
    date_reservation_start timestamp with time zone NOT NULL DEFAULT (current_timestamp AT TIME ZONE 'UTC'),
    date_reservation_end timestamp with time zone NOT NULL DEFAULT (current_timestamp AT TIME ZONE 'UTC'),
    location VARCHAR(50),
   PRIMARY KEY(id),
      CONSTRAINT fk_reservations
      FOREIGN KEY(desk_id) 
	  REFERENCES Data_Office(id)
);

