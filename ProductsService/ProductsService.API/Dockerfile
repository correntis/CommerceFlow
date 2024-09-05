
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["ProductsService/ProductsService.API/ProductsService.API.csproj", "ProductsService/ProductsService.API/"]
COPY ["CommerceFlow.Persistence/CommerceFlow.Persistence.csproj", "CommerceFlow.Persistence/"]
COPY ["CommerceFlow.Protobufs/CommerceFlow.Protobufs.csproj", "CommerceFlow.Protobufs/"]
RUN dotnet restore "ProductsService/ProductsService.API/ProductsService.API.csproj"

COPY . .
WORKDIR "/src/ProductsService/ProductsService.API"
RUN dotnet build "./ProductsService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductsService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ProductsService.API.dll"]