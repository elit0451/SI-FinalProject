FROM mcr.microsoft.com/dotnet/core/sdk:3.0

WORKDIR /app

COPY ./publish .

EXPOSE 5002

ENTRYPOINT ["dotnet", "DriverService.dll"]