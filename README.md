# first-aspnetcore-api

A simple ASP.NET Core Web API project built as my first hands-on exercise while learning ASP.NET Core.

This API simulates an in-memory employee management system with basic CRUD operations. It also demonstrates routing, handling HTTP methods, query parameters, headers, redirects, and JSON serialization/deserialization in ASP.NET Core.

**Features:**

-   List all employees
-   Retrieve an employee by ID
-   Add a new employee via JSON body
-   Update an existing employee
-   Delete an employee with basic header-based authorization
-   Redirect endpoint demonstration

**Technologies:**
ASP.NET Core, C#, System.Text.Json for JSON handling, and in-memory data storage using a List.

**How to Run:**
First, clone the repository to your local machine. Then, navigate into the project directory. Run the project using the `dotnet run` command.

Once the application is running, visit `http://localhost:5000/` in your browser to see the welcome message. You can use an API tool like Postman or cURL to interact with the following endpoints:

-   `GET /employees` to view all employees
-   `GET /employees?id=1` to view a specific employee by ID
-   `POST /employees` to add a new employee (send JSON body)
-   `PUT /employees` to update an employee (send JSON body)
-   `DELETE /employees?id=1` to delete an employee (requires an `Authorization` header set to `munna`)
-   `GET /redirect` to be redirected to `/employees`

**Note:**
This project uses in-memory storage, meaning all employee data resets when the application restarts. It was built purely for educational and practice purposes.

**License:**
This project is open-source and free to use.
