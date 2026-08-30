# Copilot Instructions for DailyReportMemoApp

## Project Overview

This project is a C# WPF desktop application named `DailyReportMemoApp`.

The purpose of the application is to record daily work logs and generate daily report memo text.

The application manages information such as:

* work date
* client/company
* project/system
* task
* memo
* multiple work time ranges per task
* total work time calculation
* special tasks not associated with a normal project task
* future monthly report output

When modifying the project, preserve the current behavior and existing design unless a change is explicitly requested.

---

## Technology Stack

Use the following stack unless explicitly instructed otherwise:

* C#
* WPF
* MVVM
* SQLite
* Entity Framework Core
* .NET
* Visual Studio 2022

Do not introduce alternative persistence mechanisms such as JSON files.

Do not introduce SQL Server, web frameworks, or unnecessary third-party libraries unless explicitly requested.

Prefer solutions using the technologies already present in the project.

---

## Project Structure

The main project folder is:

`DailyReportMemoApp/`

This is the folder that contains `DailyReportMemoApp.csproj`.

When creating or editing application source files, use paths under this project folder, not the solution folder.

The project uses or may use the following folders:

* `DailyReportMemoApp/Models`
* `DailyReportMemoApp/ViewModels`
* `DailyReportMemoApp/Views`
* `DailyReportMemoApp/Data`
* `DailyReportMemoApp/Repositories`
* `DailyReportMemoApp/Services`
* `DailyReportMemoApp/Commands`

Repository classes currently exist under `DailyReportMemoApp/Repositories`.

`Services` and `Commands` should be introduced only when they provide a clear benefit. Do not create additional architectural layers solely for architectural purity.

Do not create application source files directly next to the `.sln` file.

Before creating a new file, confirm that its location is consistent with the existing project structure.

---

## Architecture

Use MVVM as the general architectural direction, while respecting the current implementation.

Prefer the following dependency direction when applicable:

View
�� ViewModel
�� Service (when needed)
�� Repository
�� AppDbContext
�� SQLite

A Service layer is optional and should be introduced only when business logic or coordination between repositories justifies it.

Repository classes are responsible for database access.

Avoid introducing new direct `AppDbContext` access in Views or code-behind.

Existing code-behind should not be moved or extensively refactored solely to achieve stricter MVVM compliance unless explicitly requested.

The application currently uses code-behind for some dynamic WPF UI construction and event handling. Preserve these patterns when modifying related existing functionality unless a refactoring is specifically requested.

Do not perform large architectural refactorings without explicit instruction.

When making a requested change, prefer the smallest change that is clear, safe, and consistent with the existing codebase.

---

## Entity Framework Core

Use `AppDbContext` from the `DailyReportMemoApp.Data` namespace.

Use SQLite as the database provider.

The SQLite database file used by the current implementation is stored under:

`Environment.SpecialFolder.LocalApplicationData/ShigotoLog/ShigotoLog.db`

Use the existing EF Core configuration and migrations as the source of truth for the database schema.

Do not change relationships, delete behavior, constraints, or database structure unless the requested feature requires it.

### Current Entities

The project currently contains the following main entities:

* `Company`
* `Project`
* `TaskItem`
* `CompanyProject`
* `ProjectTaskItem`
* `SpecialTask`
* `WorkLog`
* `WorkTimeRange`
* `WorkingOn`

### Important Relationships and Rules

* `WorkLog` has many `WorkTimeRange` records through the `WorkTimeRanges` navigation property.
* The current `OnModelCreating` configuration uses `DeleteBehavior.Restrict` for configured entity relationships, including `WorkTimeRange -> WorkLog` and `WorkLog -> ProjectTaskItem`, `SpecialTask`, and `WorkingOn`.
* Do not assume cascade delete behavior where it is not explicitly configured.
* `WorkLog` uses the database check constraint `CK_WorkLogs_TaskType`.
* A `WorkLog` represents either a normal project task or a special task.
* `ProjectTaskItemId` and `SpecialTaskId` are mutually exclusive: one must be set while the other must be null.
* `WorkTimeRange.Duration` is a `[NotMapped]` property calculated from `EndTime - StartTime`.
* `WorkingOn` uses `DateOnly WorkDate` and tracks working start/end timestamps.

When changing entity relationships or schema-related code, consider whether an EF Core migration is required.

Do not delete or rewrite existing migrations unless explicitly requested.

---

## Repository and Database Access

The project currently uses repository classes under `DailyReportMemoApp.Repositories`.

Repositories may create and use `AppDbContext` directly following the existing project pattern.

The current implementation primarily uses synchronous EF Core operations such as:

* `SaveChanges`
* `Find`
* `FirstOrDefault`
* `ToList`

When modifying existing code, follow the existing synchronous pattern unless async behavior is specifically needed or requested.

Do not convert existing repository methods to async solely for style consistency.

When adding new queries:

* keep queries readable
* use `AsNoTracking()` for read-only operations when appropriate
* use `Include` / `ThenInclude` only for related data that is actually needed
* avoid unnecessary database calls
* preserve existing relationship and tracking behavior unless there is a reason to change it

---

## WPF UI

Follow existing WPF patterns used by the project.

The application contains both XAML-defined UI and dynamically created WPF controls.

Code-behind may be used for existing dynamic UI construction and event handling patterns.

Do not automatically rewrite working dynamic UI into XAML, commands, behaviors, or additional ViewModels unless explicitly requested.

When dynamically creating controls:

* use clear variable names
* keep layout code readable
* use `Tag` only when it is useful for associating UI elements with IDs or existing model data
* preserve existing event handling behavior
* avoid unnecessary complexity

When modifying UI behavior, prefer a focused change rather than restructuring the entire screen.

---

## Coding Style

Use clear, simple, and readable C#.

Prefer readability over cleverness or unnecessary abstraction.

Follow the style already used in the surrounding code.

Use descriptive variable and method names.

Avoid introducing abstractions, helper classes, interfaces, services, or design patterns unless they solve a concrete problem.

Use public classes for models and `AppDbContext` as required by the existing implementation.

Use namespaces that match the folder structure, including:

* `DailyReportMemoApp.Models`
* `DailyReportMemoApp.Data`
* `DailyReportMemoApp.Repositories`
* `DailyReportMemoApp.Services`
* `DailyReportMemoApp.ViewModels`
* `DailyReportMemoApp.Views`
* `DailyReportMemoApp.Commands`

Keep naming consistent with the existing codebase.

---

## Change Policy

When asked to implement or modify something:

1. Inspect the relevant existing implementation first.
2. Understand how the current code works before proposing changes.
3. Preserve existing behavior that is unrelated to the requested change.
4. Prefer the smallest reasonable modification.
5. Do not perform unrelated cleanup or refactoring.
6. Do not introduce new libraries or architectural patterns without a clear reason.
7. If a requested change affects the database schema, identify whether a migration is required.
8. If there are multiple valid approaches, prefer the approach most consistent with the existing codebase.
9. If an important design decision is ambiguous, explain the options before making a large structural change.

Do not assume that existing code should be modernized simply because a newer or more abstract approach exists.

---

## Before Editing Files

Before creating a new file, determine the intended location based on the existing project structure.

Create application files under:

`DailyReportMemoApp/`

which is the folder containing `DailyReportMemoApp.csproj`.

Do not create application source files beside the solution (`.sln`) file.

If there is ambiguity between the solution folder and project folder, use the project folder.

Before changing an existing file, inspect the surrounding implementation and keep the change consistent with it.
