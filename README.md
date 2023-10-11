# RPG_WebAPI
Web API for a simple RPG Backend.

## Features
* User Authentication
* Containerized MySQL-Database
* [Swagger Documentation](http://localhost:8082/swagger)
* Database Migrations via Entity Framework

## Run
To run the application, Docker is required to execute the following command.
```bash
docker compose up --build --remove-orphans -d
```

## Swagger Documentation
The Documentation can be accessed via
```bash
http://localhost:8082/swagger
```
After a successful Login the User can then be authorized in the top right corner by entering
```bash
bearer PUT_YOUR_HASH_HERE
```
The Hash can be found in the response body of the login request by checking the data property.

## Testing
Testing is done manually.
## 🚧 TODO
* Unit Tests
* Containerized Integration Tests