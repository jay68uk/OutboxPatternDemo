## Transactional Outbox Pattern
Implement a basic POST to add a new user (firstname, lastname, email) - constraint that email must be unique.
Uses Postgres in a Docker container for persistence.
Background worker using Quartz to handle the outbox messages.

Adding a new user will trigger a domain event(s)
Saving the user will also get the domain events and add them to the outbox table as an atomic operation.

Background worker will periodically poll the outbox table and pull out a batch of messages to process.
Depending on the type of the message a dispatcher will send the message to the appropriate handler.

### Running the demo from the terminal
- From the root folder (OutBoxPattern) run ``` docker compose up --build ```
- Navigate to the project folder ``` cd ./OutBoxPattern.Api ```
- Apply the DB migrations ``` dotnet ef database update ```
- ``` dotnet run ```
