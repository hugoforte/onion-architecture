FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy solution and projects
COPY ["Payments.sln", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["Backend/0_Core/Common/", "Backend/0_Core/Common/"]
COPY ["Backend/0_Core/Contracts/", "Backend/0_Core/Contracts/"]
COPY ["Backend/0_Core/Domain/", "Backend/0_Core/Domain/"]
COPY ["Backend/0_Core/Messaging/", "Backend/0_Core/Messaging/"]
COPY ["Backend/1_Infrastructure/Infrastructure/", "Backend/1_Infrastructure/Infrastructure/"]
COPY ["Backend/2_Application/Services.Abstractions/", "Backend/2_Application/Services.Abstractions/"]
COPY ["Backend/2_Application/Services/", "Backend/2_Application/Services/"]
COPY ["Backend/3_Run/Web/", "Backend/3_Run/Web/"]

WORKDIR "/src/Backend/3_Run/Web"

RUN dotnet restore "/src/Payments.sln" && \
    dotnet build "/src/Payments.sln" -c Release --no-restore

RUN dotnet publish "/src/Payments.sln" -c Release -o /app/publish --no-build

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5003

ENV ASPNETCORE_URLS=http://+:5003

ENTRYPOINT ["dotnet", "Starter.Web.dll"]
