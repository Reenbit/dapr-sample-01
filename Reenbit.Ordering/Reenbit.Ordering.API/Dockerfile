FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Reenbit.Ordering.API/Reenbit.Ordering.API.csproj", "Reenbit.Ordering.API/"]
RUN dotnet restore "Reenbit.Ordering.API/Reenbit.Ordering.API.csproj"
COPY . .
WORKDIR "/src/Reenbit.Ordering.API"
RUN dotnet build "Reenbit.Ordering.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Reenbit.Ordering.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 6011/tcp
ENV ASPNETCORE_URLS http://*:6011
ENTRYPOINT ["dotnet", "Reenbit.Ordering.API.dll"]