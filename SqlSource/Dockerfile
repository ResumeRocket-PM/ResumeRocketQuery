# Use the Microsoft SQL Server Developer Edition image
FROM mcr.microsoft.com/mssql/server:2022-latest

# Set environment variables
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=cf3eINo5rML5
ENV MSSQL_PID=Developer

# Expose SQL Server port
EXPOSE 1433

# Copy the SQL script that creates the 'resumerocket' user
COPY setup.sql /usr/src/setup.sql

# Run the SQL Server and execute the setup script to create 'resumerocket' user
CMD /opt/mssql/bin/sqlservr & sleep 20 && \
    /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "cf3eINo5rML5" -i /usr/src/setup.sql && \
    tail -f /dev/null
