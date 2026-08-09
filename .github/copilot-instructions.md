# Copilot Instructions for DailyReportMemoApp

## Project overview

This project is a C# WPF desktop application named DailyReportMemoApp.

The purpose of the application is to record daily work logs and generate daily report memo text.

The user wants to manage work logs with:
- work date
- client name
- system name
- task name
- memo
- multiple work time ranges per task
- total work time calculation
- future monthly report output

## Technology stack

Use the following stack unless explicitly instructed otherwise:

- C#
- WPF
- MVVM
- SQLite
- Entity Framework Core
- .NET
- Visual Studio 2022

Do not use JSON file persistence.
Do not introduce SQL Server.
Do not introduce web frameworks.
Do not introduce unnecessary third-party libraries.

## Project structure

The main project folder is:

DailyReportMemoApp/

This is the folder that contains DailyReportMemoApp.csproj.

When creating or editing application source files, use paths under this project folder, not the solution folder.

Use this folder structure:

- DailyReportMemoApp/Models
- DailyReportMemoApp/ViewModels
- DailyReportMemoApp/Views
- DailyReportMemoApp/Data
- DailyReportMemoApp/Repositories
- DailyReportMemoApp/Services
- DailyReportMemoApp/Commands

Do not create application source files directly next to the .sln file.

## Architecture rules

Follow MVVM.

Use this dependency direction:

View
-> ViewModel
-> Service
-> Repository
-> AppDbContext
-> SQLite

Do not put database access directly in Views.
Do not put database access directly in code-behind.
Avoid putting AppDbContext directly in ViewModels unless it is a temporary prototype and explicitly requested.

## Entity Framework Core rules

Use AppDbContext in the DailyReportMemoApp.Data namespace.

Use SQLite as the database provider.

The SQLite database file should be stored under:

Environment.SpecialFolder.LocalApplicationData/DailyReportMemoApp/daily_report_memo.db

Use these entities:

- WorkLog
- WorkTimeRange

WorkLog has many WorkTimeRange records.

When deleting a WorkLog, related WorkTimeRange records should be deleted by cascade delete.

## Coding style

Use clear and simple C# code.

Prefer readable code over clever code.

Use async methods for database operations when appropriate.

Use public classes for models and DbContext.

Use namespaces that match the folder structure, for example:

- DailyReportMemoApp.Models
- DailyReportMemoApp.Data
- DailyReportMemoApp.Repositories
- DailyReportMemoApp.Services
- DailyReportMemoApp.ViewModels
- DailyReportMemoApp.Commands

## Before editing files

Before creating a new file, confirm the intended path.

When asked to create a file, create it under DailyReportMemoApp/, the folder containing DailyReportMemoApp.csproj.

If there is any ambiguity between the solution folder and the project folder, choose the project folder.