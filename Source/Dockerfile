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

# Install dependencies for Chrome and ChromeDriver
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
	gnupg && \
    apt-get clean

# Install the latest stable version of Google Chrome
RUN wget -q -O google-chrome.deb https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb && \
    apt-get install -y ./google-chrome.deb && \
    rm google-chrome.deb

# Fetch the latest Chrome version number
RUN LATEST_CHROME_VERSION=$(google-chrome --version | awk '{print $3}') && \
    echo "Installed Chrome version: $LATEST_CHROME_VERSION"

# Install the latest ChromeDriver matching the Chrome version
RUN CHROME_DRIVER_VERSION=$(curl -sS https://chromedriver.storage.googleapis.com/LATEST_RELEASE) && \
    wget -q https://chromedriver.storage.googleapis.com/$CHROME_DRIVER_VERSION/chromedriver_linux64.zip && \
    unzip chromedriver_linux64.zip -d /usr/local/bin/ && \
    rm chromedriver_linux64.zip

# Add ChromeDriver to PATH
ENV PATH=$PATH:/usr/local/bin

# Set the entry point for the application
ENTRYPOINT ["dotnet", "ResumeRocketQuery.Api.dll"]
