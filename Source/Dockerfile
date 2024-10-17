# Use the official ASP.NET Core runtime as the base image for .NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET SDK 8.0 image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file to restore dependencies
COPY ["ResumeRocketQuery.sln", "."]

# Copy all project folders (including all subfolders)
COPY . .

# Restore the entire solution
RUN dotnet restore

# Set the working directory for building the specific project
WORKDIR "/src/ResumeRocketQuery.Api"  # Adjust this to the main project you want to build
RUN dotnet build "ResumeRocketQuery.Api\ResumeRocketQuery.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ResumeRocketQuery.Api\ResumeRocketQuery.Api.csproj" -c Release -o /app/publish

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResumeRocketQuery.Api.dll"]
