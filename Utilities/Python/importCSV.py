#Imports data from a CSV into a postgre database. 
#We create the table and its properties then we inject the CSV (obviously the data must correspond to the specified types)

import psycopg2
  
conn = psycopg2.connect(database="",
                        user='', password='', 
                        host='localhost', port=''
)
  
conn.autocommit = True
cursor = conn.cursor()

#(i set a drop if existing just for the test, feel free to delete this line if needed)
cleanTable = '''DROP TABLE IF EXISTS DATA;'''

cursor.execute(cleanTable)

createTableAndProps = '''CREATE TABLE DATA(id int NOT NULL,\
name char(20),\
email varchar(30), salary float);'''
  
  
cursor.execute(createTableAndProps)
  
addDataFromFile = '''COPY data(id,name,\
email,salary)
FROM 'YOURCSVPATH'
WITH DELIMITER ';'
CSV HEADER;'''
  
cursor.execute(addDataFromFile)
  
returnSelectData = '''select * from data;'''
cursor.execute(returnSelectData)
for i in cursor.fetchall():
    print(i)
  
conn.commit()
conn.close()