# TME 1

Example Application Based on technological stack as seen below:

Backend: 
MSSQL + EF Core trough IRepository (For easy DB replacement) + ASP.NET Core (REST API Server)

Frontend:
REST API Client + WPF UI

# Purpose

Purpose of this application is to present a demo project.

# Requirements

The requirements for this task are documented under the folder named Specification.

# Notes

To properly run backend tests local db is used with the following connection string:
Server=(localdb)\MSSQLLocalDB;Database=EFTestSample;Trusted_Connection=True;ConnectRetryCount=0

Otherwise to run the app it expects the connection string to be placed in secure store (or env. variable)
under the name of TME1.

Minimal testing was developed using TDD practices. By starting to implement with the most naive approach.
Throwing not implmemented exceptions.

# Projects in solution

Abstractions - Where Interfaces and Enums are defined
TestCommon - Where testing utilities reside

ServerTests - Where server tests are defined
ServerCore - Where Entity Framework Core based Database Access Persistence is implemented
ServerAPI - Where the ASP.NET Core REST API is implemented

ClientTests - Where client tests are defined
ClientCore - Where the REST API headless client is implemented
ClientApp - Where the WPF Graphical interface is implemented