
CREATE TABLE IF NOT EXISTS Users
(id INT PRIMARY KEY,
Username VARCHAR(50),
password varchar,
email VARCHAR (100)
);

CREATE TABLE reservations(
   id INT GENERATED ALWAYS AS IDENTITY,
   desk_id INT,
   userName VARCHAR(50),
    date_creation timestamp not null default CURRENT_TIMESTAMP, 
    date_reservation_start timestamp not null default CURRENT_TIMESTAMP, 
   date_reservation_end timestamp not null default CURRENT_TIMESTAMP, 
    location VARCHAR(50),
   PRIMARY KEY(id),
      CONSTRAINT fk_reservations
      FOREIGN KEY(desk_id) 
	  REFERENCES Data_Office(id)
);