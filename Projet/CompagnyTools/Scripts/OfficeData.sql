
CREATE TABLE IF NOT EXISTS Data_Office
(desk_id INT NOT NULL,
chairDirection VARCHAR(50) NOT NULL,
x int NOT NULL,
y int NOT NULL,
PRIMARY KEY(desk_id)
);

CREATE TABLE equipments(
   id INT GENERATED ALWAYS AS IDENTITY,
   desk_id INT,
   type VARCHAR(50) NOT NULL,
   specification VARCHAR(100),
   PRIMARY KEY(id),
   CONSTRAINT fk_DataOffice
      FOREIGN KEY(desk_id) 
	  REFERENCES Data_Office(desk_id)
);

INSERT INTO Data_Office (desk_id, chairDirection, x, y)
VALUES ('1', 'south', 0, 0),
 ('2', 'south', 1, 0),
 ('3', 'south', 2, 0),
 ('4', 'south', 3, 0),
 ('5', 'west', 0, 1),
 ('6', 'east', 1, 1),
 ('7', 'north-west', 2, 1),
 ('8', 'north-east', 3, 1),
 ('9', 'west', 0, 2),
 ('10', 'east', 1, 2),
 ('11', 'south-west', 2, 2),
 ('12', 'south-east', 3, 2);


 INSERT INTO equipments (desk_id, type, specification)
VALUES ('1', 'desk', 'Simple desk'),
 ('1', 'laptop', 'Laptop Dell Inspiron 15 5000'),
 ('1', 'phone', 'Cisco Phone IP 7960G/7940G'),
 ('1', 'chair', '817L Kare Ergonomic Office Chair'),
 ('1', 'drawer', 'Simple drawer');


