FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 
WORKDIR /app
COPY --from=build-env /app/publish .

EXPOSE 5002

ENTRYPOINT ["dotnet", "NotificationService.dll"]