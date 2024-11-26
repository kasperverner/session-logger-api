# Session Logger

dockerized .net core web api for logging user sessions

## Getting Started
 
make sure the database is created and migrated

```bash
dotnet ef migrations add "initial migration" -c SessionLoggerContext -s ./SessionLogger.Api -p ./SessionLogger.Persistence
dotnet ef database update -c SessionLoggerContext -s ./SessionLogger.Api -p ./SessionLogger.Persistence
```

start the api and open the Scalar UI on `http://localhost:8080/openapi/v1`

```bash
docker-compose up
```

## Structure

- Api Layer: executable application, configuration, dependency injection, and endpoints
- Application Layer: exceptions, extensions, filters, dto's, business logic, and service interfaces
- Domain Layer: entities, enums, and flags
- Infrastructure Layer: business logic, and service implementations
- Persistence Layer: database context, entity configurations, and migrations

## Stakeholder requests

### Backend

- [x] users should be assigned to a department
- [x] departments should have a hourly rates
- [x] users should contain their scheduled time off - only assignable by manager roles - purpose is explained in frontend

- [x] customers should have contacts (name, email, and role)
- [x] customers should have an override for department rate, customers can have custom rates

- [x] projects should have an optional customer reference
- [x] projects should have a state for filtering projects, that are no longer relevant
- [x] projects should have an optional approved by (contact)
- [x] projects should have an optional approved hours, to indicate how many hours the client has approved
    - [x] if approved hours is defined, sessions should not be able to exceed the approved hours
    - [x] allocated hours should be a list, as clients can approve more hours at a later point and we want to preserve a history
    - [x] an allocation should also contain an optional price as a project can be on a fixed price instead of an hourly rate
- [x] projects should have the option to be scheduled for a specific period and for a specific department (maybe this could be sprint-like)
    - [x] only users from the scheduled department, can be assigned to a task
    - [x] project schedules should be a list, as a project can be allocated for various timeslots and departments

### Frontend

- [ ] a kanban-ish board for managing tasks/projects
- [ ] a calender view for scheduled projects, with department filter
    - [ ] this will be used for scheduling new projects, to give an overview of availability
    - [ ] this view should also display employees with scheduled time off, as they cannot be assigned tasks for this period.