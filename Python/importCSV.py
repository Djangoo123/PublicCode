import psycopg2
  
conn = psycopg2.connect(database="Test",
                        user='postgres', password='root', 
                        host='localhost', port='5432'
)
  
conn.autocommit = True
cursor = conn.cursor()

cleanTable = '''DROP TABLE IF EXISTS DATA;'''

cursor.execute(cleanTable)

createTableAndProps = '''CREATE TABLE DATA(id int NOT NULL,\
name char(20),\
email varchar(30), salary float);'''
  
  
cursor.execute(createTableAndProps)
  
addDataFromFile = '''COPY data(id,name,\
email,salary)
FROM 'D:/Code/CSN/test.csv'
WITH DELIMITER ';'
CSV HEADER;'''
  
cursor.execute(addDataFromFile)
  
returnSelectData = '''select * from data;'''
cursor.execute(returnSelectData)
for i in cursor.fetchall():
    print(i)
  
conn.commit()
conn.close()