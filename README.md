# Phonebook Application

A classic CRUD Phonebook Application built using ASP.NET Core Web API, SQL Server Express, ADO.NET, Stored Procedures, and Vue.js.

## Objective

The objective of this application is to provide a simple phonebook system where users can:

- Add contacts
- View contacts
- Search contacts
- Update contacts
- Delete contacts
- Navigate contacts using server-side pagination

The application follows a monolithic architecture with ASP.NET Core handling the backend API and Vue.js providing the frontend user interface.

## Technology Stack

### Backend

- .NET 8
- ASP.NET Core Web API
- C#
- ADO.NET
- Microsoft.Data.SqlClient
- T-SQL Stored Procedures

### Database

- SQL Server Express

### Frontend

- Vue 3
- Vite
- JavaScript
- HTML
- CSS

### Development Tools

- Visual Studio 2022
- SQL Server Management Studio (SSMS)
- Node.js / npm

## Project Structure

```text
PhonebookApplication
│
├── Controllers
│   └── ContactsController.cs
│
├── Models
│   ├── Contact.cs
│   └── PagedResult.cs
│
├── Repositories
│   ├── IContactRepository.cs
│   └── ContactRepository.cs
│
├── Exceptions
│   └── DuplicatePhoneException.cs
│
├── Database
│   └── PhonebookDB.sql
│
├── wwwroot
│   ├── assets
│   ├── favicon.svg
│   ├── icons.svg
│   └── index.html
│
├── client
│   └── Vue.js frontend source
│
├── Program.cs
├── appsettings.json
└── README.md
```

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/contacts?pageNumber=1&pageSize=10` | Get paginated contacts |
| GET | `/api/contacts/{id}` | Get contact by ID |
| POST | `/api/contacts` | Add a new contact |
| PUT | `/api/contacts/{id}` | Update an existing contact |
| DELETE | `/api/contacts/{id}` | Delete a contact |

### Search

Search is supported through the `searchTerm` query parameter.

Example:

```text
GET /api/contacts?pageNumber=1&pageSize=10&searchTerm=Rahul
```
## Database Setup

1. Open SQL Server Management Studio (SSMS).
2. Connect to SQL Server Express.
3. Open the following file from the project:

```text
Database/PhonebookDB.sql
```

4. Execute the complete SQL script.

The script creates:

- `PhonebookDB` database
- `Contacts` table
- Required stored procedures for CRUD, search, and pagination

### Connection String

The application uses SQL Server Express with Windows Authentication:

```text
Server=.\SQLEXPRESS;Database=PhonebookDB;Trusted_Connection=True;TrustServerCertificate=True;
```
Make sure SQL Server Express is running before starting the application.

## How to Run the Application

### Backend and Frontend

1. Make sure SQL Server Express is running.
2. Make sure the `PhonebookDB` database has been created using the SQL script.
3. Open the project in Visual Studio 2022.
4. Build the project:

```text
dotnet build
```

5. Run the application:

```text
dotnet run
```

6. Open the application in a browser using the URL shown in the terminal.

The ASP.NET Core application serves both the Web API and the Vue.js frontend.

## Features

- Add new contacts
- View contacts in a paginated list
- Search contacts by name, phone number, or email
- Edit existing contacts
- Delete contacts
- Client-side form validation
- Server-side validation using ASP.NET Core DataAnnotations
- Database-level pagination using SQL Server `OFFSET` and `FETCH`
- Stored procedures for database operations
- Duplicate phone number validation
- RESTful API endpoints returning JSON
- Vue.js frontend served by ASP.NET Core in production