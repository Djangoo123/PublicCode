
CREATE TABLE IF NOT EXISTS DataOffice
(id INT PRIMARY KEY,
chairDirection VARCHAR(50) NOT NULL,
x int NOT NULL,
y int NOT NULL,
officeData json NOT NULL
);

INSERT INTO DataOffice (id, chairDirection, x, y, officeData)
VALUES ('1', 'south', 0, 0, '{"type": "desk", "specification": "Simple desk", "type": "laptop", "specification": "Laptop Dell Inspiron 15 5000", "type": "phone", "specification": "Cisco Phone IP 7960G/7940G", "type": "chair", "specification": "817L Kare Ergonomic Office Chair", "type": "drawer", "specification": "Simple drawer"}'),
 ('2', 'south', 1, 0, '{"type": "desk", "specification": "Simple desk", "type": "laptop", "specification": "Laptop Dell Inspiron 15 5000", "type": "phone", "specification": "Cisco Phone IP 7960G/7940G", "type": "chair", "specification": "817L Kare Ergonomic Office Chair", "type": "drawer", "specification": "Simple drawer"}'),
 ('3', 'south', 2, 0, '{"type": "desk", "specification": "Simple desk", "type": "laptop", "specification": "Laptop Dell Inspiron 15 5000", "type": "phone", "specification": "Cisco Phone IP 7960G/7940G", "type": "chair", "specification": "817L Kare Ergonomic Office Chair", "type": "drawer", "specification": "Simple drawer"}'),
 ('4', 'south', 3, 0, '{"type": "desk", "specification": "Simple desk", "type": "laptop", "specification": "Laptop Dell Inspiron 15 5000", "type": "phone", "specification": "Cisco Phone IP 7960G/7940G", "type": "chair", "specification": "817L Kare Ergonomic Office Chair", "type": "drawer", "specification": "Simple drawer"}'),
 ('5', 'west', 0, 1, '{"type": "desk", "specification": "Simple desk", "type": "laptop", "specification": "Laptop Dell Inspiron 15 5000", "type": "phone", "specification": "Cisco Phone IP 7960G/7940G", "type": "chair", "specification": "817L Kare Ergonomic Office Chair", "type": "drawer", "specification": "Simple drawer"}'),
 ('6', 'east', 1, 1, '{"type": "desk", "specification": "Simple desk", "type": "laptop", "specification": "Laptop Dell Inspiron 15 5000", "type": "phone", "specification": "Cisco Phone IP 7960G/7940G", "type": "chair", "specification": "817L Kare Ergonomic Office Chair", "type": "drawer", "specification": "Simple drawer"}'),
 ('7', 'north-west', 2, 1, '{"type": "desk", "specification": "Simple desk", "type": "laptop", "specification": "Laptop Dell Inspiron 15 5000", "type": "phone", "specification": "Cisco Phone IP 7960G/7940G", "type": "chair", "specification": "817L Kare Ergonomic Office Chair", "type": "drawer", "specification": "Simple drawer"}'),
 ('8', 'north-east', 3, 1, '{"type": "desk", "specification": "Simple desk", "type": "laptop", "specification": "Laptop Dell Inspiron 15 5000", "type": "phone", "specification": "Cisco Phone IP 7960G/7940G", "type": "chair", "specification": "817L Kare Ergonomic Office Chair", "type": "drawer", "specification": "Simple drawer"}'),
 ('9', 'west', 0, 2, '{"type": "desk", "specification": "Simple desk", "type": "laptop", "specification": "Laptop Dell Inspiron 15 5000", "type": "phone", "specification": "Cisco Phone IP 7960G/7940G", "type": "chair", "specification": "817L Kare Ergonomic Office Chair", "type": "drawer", "specification": "Simple drawer"}'),
 ('10', 'east', 1, 2, '{"type": "desk", "specification": "Simple desk", "type": "laptop", "specification": "Laptop Dell Inspiron 15 5000", "type": "phone", "specification": "Cisco Phone IP 7960G/7940G", "type": "chair", "specification": "817L Kare Ergonomic Office Chair", "type": "drawer", "specification": "Simple drawer"}'),
 ('11', 'south-west', 2, 2, '{"type": "desk", "specification": "Simple desk", "type": "laptop", "specification": "Laptop Dell Inspiron 15 5000", "type": "phone", "specification": "Cisco Phone IP 7960G/7940G", "type": "chair", "specification": "817L Kare Ergonomic Office Chair", "type": "drawer", "specification": "Simple drawer"}'),
 ('12', 'south-east', 3, 2, '{"type": "desk", "specification": "Simple desk", "type": "laptop", "specification": "Laptop Dell Inspiron 15 5000", "type": "phone", "specification": "Cisco Phone IP 7960G/7940G", "type": "chair", "specification": "817L Kare Ergonomic Office Chair", "type": "drawer", "specification": "Simple drawer"}');
