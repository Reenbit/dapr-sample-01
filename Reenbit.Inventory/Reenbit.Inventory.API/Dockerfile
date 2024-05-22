FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Reenbit.Inventory.API/Reenbit.Inventory.API.csproj", "Reenbit.Inventory.API/"]
COPY ["nuget.config", "./"]
RUN dotnet restore "Reenbit.Inventory.API/Reenbit.Inventory.API.csproj"
COPY . .
WORKDIR "/src/Reenbit.Inventory.API"
RUN dotnet build "Reenbit.Inventory.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Reenbit.Inventory.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 6012/tcp
ENV ASPNETCORE_URLS http://*:6012
ENTRYPOINT ["dotnet", "Reenbit.Inventory.API.dll"]