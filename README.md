# WebApplicationAssessment

## Prerequisites

* **Database:** SQL Server
* **Framework:** .NET / ASP.NET Core
* **IDE:** Visual Studio

I chose **SQL Server** because of its deep integration with the .NET ecosystem and tooling through Visual Studio.

---

## Configure Connection String

Open the `appsettings.Development.json` file and replace the database host with `localhost`.

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=WebApplicationAssessment;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> Update the connection string according to your local SQL Server configuration.

---

## Migrations

Database migrations are configured to run automatically.

Simply:

1. Clone the repository.
2. Configure the SQL Server connection string.
3. Start the project.
4. The required database migrations will be applied automatically.

Once the application starts, you can see one from below urls:

* `https://localhost:7042`
* `http://localhost:5051`

---

# Code Explanation & Design Decisions

## Project Structure

The project follows a standard **MVC / Clean Architecture** pattern to ensure separation of concerns.

### Controllers / Endpoints

Handle incoming HTTP requests, routing, and API endpoint operations.

### Services

Contain the application's business logic and keep controllers lightweight and focused on handling requests.

### Data Access

Responsible for:

* Entity Framework Core `DbContext`
* Database access
* Entity configurations

### Migrations

* Database migrations

---

## Employee–Skill Relationship Modeling

The relationship between **Employees** and **Skills** is modeled as a **Many-to-Many relationship**.

A joining entity/table called `EmployeeSkill` is used to map the relationship between employees and skills.

The `EmployeeSkill` entity contains:

* `EmployeeId`
* `SkillId`

In Entity Framework Core, this relationship is configured using the **Fluent API** in the `DbContext`.

This approach:

* Maintains referential integrity.
* Clearly represents the many-to-many relationship.
* Makes it easy to query an employee's skills.
* Makes it easy to query employees associated with a particular skill.

---

## Validation Implementation

Data validation is implemented using **Data Annotations** and **Custom Data Annotations**.

The model-level validation includes:

* Required fields
* String length restrictions
* Email format validation
* Other applicable field-level validations

This helps ensure that invalid data is rejected before it reaches the business logic or database layer.

---

# Future Improvements

Given more time, I would improve the project by adding the following:

### 1. Unit and Integration Testing

Implement unit and integration tests using **xUnit** to ensure business logic reliability and prevent regressions.

### 2. Authentication / Authorization

Secure the API endpoints using **JWT Authentication** or **ASP.NET Core Identity** so that only authorized users can create, update, or delete employee records.

### 3. Global Exception Handling

Add custom middleware for global exception handling to:

* Catch unhandled exceptions.
* Log errors consistently.
* Return standardized error responses.
* Avoid exposing unnecessary internal implementation details.

### 4. Generic / Reusable Code

Identify duplicate code across the application and introduce generic or reusable solutions where appropriate.

This would improve:

* Maintainability
* Code reusability
* Consistency
* Overall development efficiency

### 5. Pagination & Filtering

Implement pagination and filtering when retrieving employees.

This would improve List or Grid performance and reduce the amount of data returned when working with large employee datasets.
