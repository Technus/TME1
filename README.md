# TME 1

Example Application Based on technological stack as seen below:

Backend: 
 - MSSQL Database
 - EF Core trough IRepository (For easy DB replacement)
 - ASP.NET Core (REST API Server)

Frontend:
 - REST API Client
 - WPF UI

# Purpose

Purpose of this application is to present a demo project.

# Requirements

The requirements for this task are documented under the folder named Specification.

# Notes

To properly run backend tests local db is used with the following connection string:
`Server=(localdb)\MSSQLLocalDB;Database=EFTestSample;Trusted_Connection=True;ConnectRetryCount=0`

To properly run backend in dev local db is used with the following connection string:
`Server=(localdb)\MSSQLLocalDB;Database=EFDevSample;Trusted_Connection=True;ConnectRetryCount=0`

Otherwise to run the app it expects the connection string to be placed in config (or env. variable) under the name of TME1.

Tests were developed using TDD practices. By starting with the most naive approach.
`Throwing not implmemented exceptions.` And then building up the functionality.

The Architecture is far from perfect. As it was designed on the fly for excercise.
For Proper Clean Architecture see: 

https://github.com/search?q=clean+architecture+language%3AC%23&type=repositories&l=C%23&s=stars&o=desc

Project was supposed to include pagination of data, which is a rather simple concept. (Cursor/Skip/Take)
Instead due to lack of time performace to some extent can be expected thanks to `Virtualizing UI controls`.
Which were used before specification update.

Troughout the solution there are many `Fin-s` but no fish. 
It is how the result pattern is called in the external library. (It also has `Result` but for internal use only...)

# Projects in solution

 - Abstractions - Where common Interfaces and Enums are defined
 - TestCommon - Where testing utilities reside

 - ServerTests - Where server tests are defined
 - ServerCore - Where Entity Framework Core based Database Access Persistence is implemented
 - ServerAPI - Where the ASP.NET Core REST API is implemented

 - ClientTests - Where client tests are defined
 - ClientCore - Where the REST API headless client is implemented
 - ClientApp - Where the WPF Graphical interface is implemented
