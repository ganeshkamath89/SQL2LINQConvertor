# SQL2LINQConvertor

Hi I am Ganesh Kamath. I have stopped working on this project and have moved on to a different company.

I have created a basic SQL to LINQ convertor which I hope to see compete in converting SQL to LINQ as the project progresses.

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

Environment Setup:
1) Visual Studio 2015 Community Edition
2) Microsoft Modelling and Visualization SDK
3) Entity Framework 6 and EF6 Tools

Add 2 environment Variables:
1) Add the following path to Path, so that the TextTransform.exe path is recognized:
C:\Program Files (x86)\Common Files\Microsoft Shared\TextTemplating\14.0\
2) Add the following 

[Variable] VS140COMNTXTTRANSFORM

[Value] C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\Microsoft\Entity Framework Tools\Templates\Includes

Remove the first line from the following file:

C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\Microsoft\Entity Framework Tools\Templates\Includes\EF6.Utility.CS.ttinclude

Otherwise it gives error (I dont remember the error or the line that was there).


Preferable run as administrator.

![sql2linq](https://user-images.githubusercontent.com/2648522/34672707-8382793e-f4a5-11e7-9061-b48106e65009.jpg)

I came across several complex SQL Queries with Subqueries and Group by clauses which I aim at implementing some time from now. But for the purpose of early feedback I will be posting my code as it is (by chaning very few details in the code in order to generalize). I have explicitly verified from our company regarding posting this code.

Feel free to fork a copy and make the changes relevent to your application.
