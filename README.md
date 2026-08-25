# tds-backend-api

Personal Assistant API (ASP.NET). Auth (JWT), todos, folders/files. Frontend: separate Next.js repo.

## Fast path / entrypoint map

```
UI / HTTP
  → Controllers/*Controller.cs
  → Services/*Service.cs
  → AppDbContext (EF Core)
  → SQLite file App/tds.db   [current]
  → disk App/uploads/{userId}/   [current file bytes]

Auth: AuthController → JwtTokenService (+ UserService on register/me)
Files: FileController → FileService → DB metadata + local disk
Folders: FolderController → FolderService → DB tree only (no mirror on disk)
Owner repair: OwnerController → OwnerService.CreateUserRootFolder
```

**Preferred path now:** SQLite + local `uploads/` (hardwired in `Program.cs` / `FileService`).

## WIP

Not implemented yet — roadmap only:

- **DB:** provider selectable via config (`Sqlite` | `Postgres` | …), not only `UseSqlite` in `Program.cs`.
- **File storage:** abstraction over local disk; keep local uploads for now; later a worker/queue for object storage (e.g. AWS S3).

## Requirements

- .NET 10 SDK
- (optional) `dotnet tool install -g dotnet-ef` for migrations

## Run

```bash
cd App
dotnet restore
dotnet ef database update --project App.csproj   # if DB missing / schema changed
dotnet run --launch-profile http
```

API: `http://localhost:5289`

Config (dev): `App/appsettings.Development.json` — `Jwt:*`, `ConnectionStrings:Default`.

Do not commit real secrets; keep local overrides in ignored `appsettings.*.local.json` or env vars.

## Smoke auth

```bash
# register
curl -s -X POST http://localhost:5289/api/auth/register \
  -H 'Content-Type: application/json' \
  -d '{"username":"demo","password":"demo-pass-123"}'

# login → accessToken
curl -s -X POST http://localhost:5289/api/auth/login \
  -H 'Content-Type: application/json' \
  -d '{"username":"demo","password":"demo-pass-123"}'

# me
curl -s http://localhost:5289/api/auth/me -H "Authorization: Bearer <token>"
```

After changing JWT claims (e.g. `root_folder_id`), re-login.

## Tests

```bash
dotnet test App.Tests/App.Tests.csproj
```

Scaffold only for now — expand later.

## Invariants / gotchas

- Folders = DB metadata; file bytes = `uploads/{userId}/{fileId}` (flat per user). Move folder/file = change `ParentId` only.
- `GetUser` builds `UserResponse` from JWT claims (incl. `root_folder_id`); stale tokens until re-login.
- Root folder: cannot delete; register creates it; owner endpoint can repair DB/disk mismatch.
- Errors: services return `ServiceResult<T>` → controllers `ToActionResult` (`ServiceResultHttp.cs`).
- Gitignored: `bin/`, `obj/`, `*.db`, `uploads/`, `main.md`.

## Search anchors

- `ServiceResult` / `ToActionResult`
- `root_folder_id`
- `CreateUserRootFolder`
- `StoragePath`
