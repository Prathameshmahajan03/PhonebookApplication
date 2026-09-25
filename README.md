# Phonebook Application

A working CRUD phonebook built with an ASP.NET Core Web API, SQL Server Express, ADO.NET, stored procedures, and a Vue 3 SPA.

## Architecture

This repository is organized as a monorepo:

- `backend/` is a pure .NET 8 ASP.NET Core REST API. It does not serve Vue files.
- `frontend/` is the Vue 3/Vite SPA. Vite builds it to `frontend/dist`.
- The database is initialized separately from `backend/Database/PhonebookDB.sql`.

The frontend and API use the existing `/api/contacts` contract. In development, Vite proxies `/api` to the HTTPS backend. In production, host `frontend/dist` separately and route `/api/*` to the backend through a same-origin reverse proxy, or configure an explicit cross-origin API policy separately.

## Project structure

```text
PhonebookApplication/
├── PhonebookApplication.sln
├── README.md
├── AGENTS.md
├── backend/
│   ├── PhonebookApplication.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── PhonebookApplication.http
│   ├── Controllers/
│   ├── Models/
│   ├── Repositories/
│   ├── Exceptions/
│   ├── Database/
│   │   └── PhonebookDB.sql
│   └── Properties/
│       └── launchSettings.json
└── frontend/
    ├── package.json
    ├── package-lock.json
    ├── vite.config.js
    ├── index.html
    ├── src/
    ├── public/
    ├── .vscode/
    └── README.md
```

`backend/bin`, `backend/obj`, `frontend/node_modules`, and `frontend/dist` are generated and ignored.

## Technology stack

### Backend

- .NET 8 / ASP.NET Core Web API
- C# controller-based REST API
- ADO.NET with `Microsoft.Data.SqlClient`
- SQL Server Express
- T-SQL stored procedures
- Swashbuckle/OpenAPI

### Frontend

- Vue 3
- Vite
- JavaScript and Vue Single-File Components
- Global CSS

## Database setup

1. Start SQL Server Express.
2. Open SSMS and run `backend/Database/PhonebookDB.sql` against the intended SQL Server instance.
3. Run `backend/Database/Authentication.sql` against the same database.
4. Confirm the `PhonebookDB` database, `Contacts` and `Users` tables, and all required stored procedures exist.

The script creates the database only when it is absent; its table and procedure creation statements are not idempotent. Do not run the complete script against an already initialized database as a routine update. The application does not create or migrate the database at startup.

The default local connection string is in `backend/appsettings.json` and targets SQL Server Express with Windows integrated authentication.

### Initial user provisioning

The API provisions the configured initial user once at application startup. The username is configured in `backend/appsettings.json`. Configure the initial password through User Secrets or an environment variable; do not add the password to source control or SQL scripts:

```text
dotnet user-secrets set "InitialUser:Password" "<initial-user-password>"
```

## Development

Install and build each application independently.

Backend:

```text
dotnet restore PhonebookApplication.sln
dotnet build PhonebookApplication.sln
```

Start the backend with the HTTPS profile used by the Vite proxy:

```text
dotnet run --project backend/PhonebookApplication.csproj --launch-profile https
```

The API is available at `https://localhost:7233`; Swagger is available at `/swagger` in the Development environment.

Frontend, in a second terminal:

```text
npm --prefix frontend ci
npm --prefix frontend run dev
```

Vite serves the SPA and proxies `/api` requests to `https://localhost:7233`. The proxy is defined in `frontend/vite.config.js`.

## Frontend build

```text
npm --prefix frontend run build
```

The deployable frontend artifact is `frontend/dist`. ASP.NET Core does not copy or serve this directory.

## Backend publish

```text
dotnet publish backend/PhonebookApplication.csproj -c Release -o <publish-output>
```

The backend publish output contains the REST API and its configuration, not the Vue application.

## API endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/contacts?pageNumber=1&pageSize=10&searchTerm=` | Paginated contacts and server-side search |
| GET | `/api/contacts/{id}` | Get one contact |
| POST | `/api/contacts` | Create a contact; returns `201` |
| PUT | `/api/contacts/{id}` | Update a contact; returns `200` |
| DELETE | `/api/contacts/{id}` | Delete a contact; returns `204` |

Contacts are validated with the existing rules: required name, exactly 10-digit phone number, optional email, and maximum 255-character name/email values. The existing duplicate-phone response and stored procedure behavior are unchanged.

## Features

- Add contacts
- View contacts in a paginated list
- Search by name, phone number, or email
- Edit existing contacts
- Delete contacts
- Client-side and ASP.NET Core DataAnnotations validation
- Database-level pagination using SQL Server `OFFSET/FETCH`
- Stored procedures for CRUD, search, and pagination
- Duplicate phone validation
- RESTful JSON API

## Testing and deployment notes

This repository has no automated backend or frontend test suite. Use the build commands above and manually verify the API/UI workflows against a disposable or backed-up database before running mutating requests.

No Docker, CI/CD, reverse-proxy, or production hosting configuration is included. The production host must serve `frontend/dist` and route `/api/*` to the backend if the frontend is to use its existing relative URLs.
