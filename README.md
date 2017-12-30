# SQL2LINQConvertor
A work in progress project aimed at conversion of SQL queries to LINQ queries

Hi I am Ganesh Kamath. I work for ERachana Technologies.

I have been working on ASP.net applications for the past 7 months. I am still new to SQL and LINQ. I have created a basic SQL to LINQ convertor which I hope to see compete in converting SQL to LINQ as the project progresses.

Between December 19th and December 30th, I has asked permission to work on this SQL To Linq Convertor at work. I want my 2 week old  project to gain more contributors over time in order to help others like me who might be looking for a solution. I want this code to act as a plugin where user can paste SQL on the Left Hand side to see LINQ  on the Right Hand Side on the click of a button.

As of now the code is written to handle simple Select statements.

Feature List - only simple select statements:
1) Column name is specified from single table.
2) Column name is specified from Join Tables.
3) Where Clause is written with simple logical check separated by And
4) user defined columns mixed along with Table defined columns
5) simple Case-When-Then-End condition
6) Order by when single condition is used.

Work pending - handle more complex SQL:
1) Handle nesting
2) Handle sub queries
3) Handle advanced queries




I came across several complex SQL Queries with Subqueries and Group by clauses which I aim at implementing some time from now. But for the purpose of early feedback I will be posting my code as it is (by chaning few details in the code). I have explicitly verified from our company regarding posting this code.

If anyone else wishes to contribute here. You are welcome to drop me an email at "ganeshkamath89@gmail.com" or contact me at my LinkedIn ID: https://www.linkedin.com/in/ganeshkamath89
