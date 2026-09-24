# Repository instructions

## Current boundaries and entry points

- `PhonebookApplication.sln` references the single project `backend/PhonebookApplication.csproj`.
- ASP.NET Core starts in `backend/Program.cs`; all API routes are in `backend/Controllers/ContactsController.cs`.
- The Vue app is an independent npm package under `frontend/`; its entrypoint is `frontend/src/main.js` and its root component is `frontend/src/App.vue`.
- The backend is API-only: it no longer calls `UseDefaultFiles`, `UseStaticFiles`, or `MapFallbackToFile`, and there is no root `wwwroot` SPA copy.
- There are no test, CI, Docker, or deployment projects in this repository.

## Toolchain and prerequisites

- Backend target is `net8.0`; no `global.json` pins the SDK. Direct packages are `Microsoft.Data.SqlClient` 7.1.0 and `Swashbuckle.AspNetCore` 6.6.2.
- Vite 8 and `@vitejs/plugin-vue` 6 require Node `^20.19.0 || >=22.12.0`; no Node version is pinned.
- Local database access assumes SQL Server Express instance `.\SQLEXPRESS`, database `PhonebookDB`, and Windows integrated authentication from `backend/appsettings.json`.
- The API can start without a working database; SQL/procedure failures appear on the first database-backed request.

## Commands from the repository root

- Install locked frontend dependencies: `npm --prefix frontend ci`
- Start Vite: `npm --prefix frontend run dev`
- Build the SPA: `npm --prefix frontend run build`
- Restore/build backend: `dotnet restore PhonebookApplication.sln` then `dotnet build PhonebookApplication.sln`
- Start the API on the Vite target port: `dotnet run --project backend/PhonebookApplication.csproj --launch-profile https`
- The HTTPS profile is `https://localhost:7233`; use it explicitly because the first HTTP profile is port `5127`.
- Swagger is available at `https://localhost:7233/swagger` in Development.
- There is no backend test project, frontend test script, lint task, formatter task, or typecheck task. Do not claim `dotnet test` or `npm test` validates this repository.
- `npm run preview` does not inherit the dev-server proxy because `frontend/vite.config.js` configures `server.proxy`, not `preview.proxy`.

## Frontend/API build and communication

- Vite writes to `frontend/dist`; ASP.NET Core does not build, copy, or serve the SPA.
- Do not edit generated `frontend/dist` files as a substitute for source changes.
- The Vite assets currently use root-absolute `/assets/...` and `/favicon.svg` paths; subpath hosting requires changing Vite `base`.
- The browser uses relative `/api/contacts` URLs. Development Vite proxies `/api` to `https://localhost:7233` with `changeOrigin: true` and `secure: false`.
- Production needs a static host for `frontend/dist` plus a same-origin `/api` reverse proxy, unless an explicit cross-origin API policy is separately added.

## API contract to preserve unless the task explicitly changes it

- Routes are `/api/contacts` and `/api/contacts/{id}`; the controller template generates `/api/Contacts`, but ASP.NET Core routing is case-insensitive.
- `GET /api/contacts` accepts `pageNumber=1`, `pageSize=10`, and nullable `searchTerm`; it returns `items`, `totalCount`, `currentPage`, `pageSize`, and `totalPages`.
- `POST` returns `201` with the created contact and `Location`; `PUT` returns `200` with an empty body; `DELETE` returns `204`; missing update/delete/get targets return `404`.
- Duplicate phone numbers return plain-text `400` containing `Phone number already exists.`
- `Contact` is both request and response model. Body `id` and `createdAt` are accepted but ignored by insert/update calls.
- The frontend depends on camel-case JSON and exact contact/pagination property names.
- HTTP validation requires a name of at most 255 characters, a 10-digit phone, and an optional valid email of at most 255 characters.
- The frontend does not call `GET /api/contacts/{id}`; edit uses the list object.
- Generated metadata under `backend/obj/.../EndpointInfo/PhonebookApplication.json` can be stale; controller source is authoritative for statuses.
- The backend no longer falls back to `index.html`; an unknown API path should not be treated as a successful Vue route.

## Database contract

- `backend/Database/PhonebookDB.sql` is manually executed in SSMS; the API does not initialize or migrate the database.
- Only database creation is guarded; the table and all five procedures are unconditional, so do not run the full script against an initialized database as a routine update.
- Exact procedure names are `sp_GetContactsPaged`, `sp_GetContactById`, `sp_InsertContact`, `sp_UpdateContact`, and `sp_DeleteContact`.
- `ContactRepository` maps exact result columns `Id`, `Name`, `PhoneNumber`, `Email`, `Address`, `CreatedAt`; the paged procedure returns contacts first and `TotalCount` second.
- Search covers name, phone, and email with leading-wildcard `LIKE`, orders by name, and uses `OFFSET/FETCH`; address is not searched.
- Database phone storage is `NVARCHAR(50) UNIQUE`, while HTTP validation requires exactly 10 digits. Preserve or deliberately reconcile this mismatch when changing validation/schema.
- There are no EF migrations, seed scripts, rollback scripts, foreign keys, or database integration tests.

## Scope and safety notes

- Do not change database schema/procedure names, stored procedure contracts, CRUD behavior, validation, search, pagination, JSON property names, or ADO.NET/repository structure during a directory-only restructuring.
- Do not add EF Core, Dapper, authentication, authorization, Vue Router, Pinia, TypeScript, Docker, or unrelated features without an explicit request.
- Keep `Microsoft.NET.Sdk.Web`; the project is already a controller-based REST API.
- `AllowedHosts: "*"` is not CORS.
- Treat `backend/PhonebookApplication.csproj`, `backend/Program.cs`, controller/repository/SQL files, `frontend/package.json`, and `frontend/vite.config.js` as executable sources of truth when prose conflicts.
