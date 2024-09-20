# Civitta-task

## Create a small WEB API application that returns country public holidays.

### Requirements for the Application:
1. Endpoints to implement:
- countries list
- grouped by a month holidays list for a given country and year
- specific day status(workday, free day, holiday)
- the maximum number of free(free day + holiday) days in a row, which will be by a given country and year
2. Endpoints should return JSON as a response.
3. To get a data(countries, holidays) your application should use a JSON API from https://kayaposoft.com/enrico/. Your application should avoid repeated requests to a https://kayaposoft.com/enrico/. Results from a https://kayaposoft.com/enrico/ should be normalized and saved to a database, so next time your application should query a database.
 
### Requirements:
1. Use .NET 5 or newer version of the framework
2. README.md with deployment instructions
3. Database - MSSQL
4. Version control - GIT(bitbucket, github or gitlab)
5. API should have automatically generated OpenAPI documentation
6. Deployment - the project must be placed on the internet and publicly available (e.g. free hosting available at Azure (12 months free tier))
7. Testing - URLs smoke tests, Unit tests
8. Local development environment - docker based
 
