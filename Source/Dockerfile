# Use the official ASP.NET Core runtime as the base image for .NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT=Production

# Use the .NET SDK 8.0 image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Copy the solution file to restore dependencies
COPY ["ResumeRocketQuery.sln", "."]

# Copy all project folders (including all subfolders)
COPY . .

# Restore the entire solution
RUN dotnet restore

# Set the working directory for building the specific project
RUN dotnet build "ResumeRocketQuery.Api/ResumeRocketQuery.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ResumeRocketQuery.Api/ResumeRocketQuery.Api.csproj" -c Release -o /app/publish

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install necessary dependencies
RUN apt-get update && apt-get install -y \
    wget \
    curl \
    unzip \
    ca-certificates \
    libnss3 \
    libgdk-pixbuf2.0-0 \
    libatk-bridge2.0-0 \
    libatk1.0-0 \
    libx11-xcb1 \
    libxcomposite1 \
    libxdamage1 \
    libxrandr2 \
    libappindicator3-1 \
    libgbm1 \
    libasound2 \
    fonts-liberation \
    libappindicator1 \
    libnspr4 \
    libnss3 \
    lsb-release \
    xdg-utils \
    libxss1 \
    libxtst6 \
    libgtk-3-0 && \
    apt-get clean

# Install Chrome 114
RUN wget -q https://dl.google.com/linux/chrome/deb/pool/main/g/google-chrome-stable/google-chrome-stable_114.0.5735.90-1_amd64.deb && \
    apt-get install -y ./google-chrome-stable_114.0.5735.90-1_amd64.deb && \
    rm google-chrome-stable_114.0.5735.90-1_amd64.deb

# Install ChromeDriver for version 114
RUN wget -q https://chromedriver.storage.googleapis.com/114.0.5735.90/chromedriver_linux64.zip && \
    unzip chromedriver_linux64.zip -d /usr/local/bin/ && \
    rm chromedriver_linux64.zip

# Set environment variable for ChromeDriver
ENV PATH=$PATH:/usr/local/bin

# Set the entry point for the application
ENTRYPOINT ["dotnet", "ResumeRocketQuery.Api.dll"]
