FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY ["ScavengeRUs/ScavengeRUS.db", "."]

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ScavengeRUs/ScavengeRUs.csproj", "ScavengeRUs/"]
RUN dotnet restore "ScavengeRUs/ScavengeRUs.csproj"
COPY . .
WORKDIR "/src/ScavengeRUs"
RUN dotnet build "ScavengeRUs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScavengeRUs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ScavengeRUs.dll"]
