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
 
## Installation

1. Clone the repository
`git clone https://github.com/yourusername/your-repo-name.git
cd your-repo-name`
2. Install dependencies
  * Ensure you have .NET SDK installed on your machine. You can download it from [here](https://dotnet.microsoft.com/en-us/download/dotnet).
  * Ensure you have Docker installed on your machine. You can download it form [here](https://www.docker.com/products/docker-desktop/).

For deployment first time I tried to use https://railway.app/ deployment environment, but docker file build fails. Locally everything works.
For deployment second time I tried to use azure https://publicholidayapi-c6cwf3hhbrenc5ff.northeurope-01.azurewebsites.net/swagger/index.html development environment, but database not load, but api correctly deployed.

For final deployment used friend server https://publicholidayapi.amperelink.com/swagger/index.html and everything works.

## Deployment instructions

1. Build the Docker Image: Before running the application, build the Docker image. This step is necessary if your docker-compose.yml uses a local Dockerfile to build the image:
`docker-compose build`
2. Run the Application: Once the image is built, run your application in detached mode (-d flag runs the containers in the background): `docker-compose up -d`
Docker Compose will:
* Build and start all the services defined in your docker-compose.yml.
* Keep them running in the background.
3. Verify the Deployment: To check if everything is running correctly, you can use: `docker-compose ps`
4. Access the Application: Open your browser and go to the specified port. For example, if your docker-compose.yml maps port 8080, visit: `http://localhost:8080`
5. Stopping the Application: When you want to stop the services, run: `docker-compose down`

