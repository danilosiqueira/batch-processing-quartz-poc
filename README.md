# Batch Processing Quartz POC

This project is a minimal-API lab demonstrating asynchronous batch processing with ASP.NET Core, Quartz.NET, and PostgreSQL. The devcontainer already provides the runtime environment and a PostgreSQL service via its docker-compose, so no extra compose file is needed in this repo.

## Running inside the devcontainer
1. Open the workspace in VS Code (Dev Containers extension will start the container).
2. The devcontainer docker-compose brings up PostgreSQL automatically. Default connection variables:
   - Host: `postgres`
   - Port: `5432`
   - Database: `batch_db`
   - User: `batch_user`
   - Password: `batch_password`
3. The API reads the connection string from `ConnectionStrings:BatchDb` or `POSTGRES_CONNECTION`. Example:
   ```bash
   export POSTGRES_CONNECTION="Host=postgres;Port=5432;Database=batch_db;Username=batch_user;Password=batch_password"
   dotnet run --project BatchProcessingQuartzPoc
   ```

## API quickstart
- POST `/batch-actions` with body `{ "items": ["item-1", "item-2"] }` to enqueue processing.
- GET `/batch-actions/{id}` to check overall and per-item status.

## Notes
- Quartz is configured with one durable job (`ProcessItemJob`) and one trigger per item.
- Handlers are selected by `ActionType`; `ProcessItemHandler` is the sample handler.
- EF Core uses `EnsureCreated` at startup for simplicity; switch to migrations if you prefer schema evolution.
