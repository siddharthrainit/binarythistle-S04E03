# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /platformApp

# Copy csproj and restore as distinct layer
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build app
COPY . ./
RUN dotnet publish  -c Release -o out

# Final staget/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /platformApp
COPY --from=build-env /platformApp/out .

ENTRYPOINT ["dotnet", "CommandsService.dll"]


#docker build -t siddharthrainit/commandsservice .
#docker run -p 8000:8080 -d siddharthrainit/commandsservice 
#8000: The port on the host machine that maps to the container's port 8080.
#Port 8080: The port inside the container where the application is running (container's exposed port).
#Port 8000: The port on the host machine that maps to the container's port 8080.
#docker push siddharthrainit/commandsservice