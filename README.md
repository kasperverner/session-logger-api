# Session Logger

dockerized .net core web api for logging user sessions

## Getting Started
 
make sure the database is created and migrated

```bash
dotnet ef migrations add "initial migration" -c SessionLoggerContext -s ./SessionLogger.Api -p ./SessionLogger.Persistence
dotnet ef database update -c SessionLoggerContext -s ./SessionLogger.Api -p ./SessionLogger.Persistence
```

start the api and open the swagger ui on `http://localhost:8080/swagger`

```bash
docker-compose up
```

## Structure

- Api Layer: executable application, configuration, dependency injection, and endpoints
- Application Layer: exceptions, extensions, filters, dto's, business logic, and service interfaces
- Domain Layer: entities, enums, and flags
- Infrastructure Layer: business logic, and service implementations
- Persistence Layer: database context, entity configurations, and migrations

## TODO

- [ ] Add filtering on sessions, projects and tasks
- [ ] Add cron jobs
- [ ] Add unit tests
- [ ] Add integration tests