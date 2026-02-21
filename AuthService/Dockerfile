# -------- BUILD STAGE --------
    FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
    WORKDIR /src
    
    # Copy csproj and restore dependencies
    COPY ["AuthService.csproj", "./"]
    RUN dotnet restore
    
    # Copy everything else and build
    COPY . .
    RUN dotnet publish -c Release -o /app/publish
    
    # -------- RUNTIME STAGE --------
    FROM mcr.microsoft.com/dotnet/aspnet:9.0
    WORKDIR /app
    
    # Expose port 8080 (used in docker-compose)
    EXPOSE 8080
    
    # Copy published files from build stage
    COPY --from=build /app/publish .
    
    # Run the app
    ENTRYPOINT ["dotnet", "AuthService.dll"]