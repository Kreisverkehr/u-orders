#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["UOrders.Service/UOrders.Service.csproj", "UOrders.Service/"]
COPY ["UOrders.EFModel/UOrders.EFModel.csproj", "UOrders.EFModel/"]
COPY ["UOrders.DTOModel/UOrders.DTOModel.csproj", "UOrders.DTOModel/"]
COPY ["UOrders.EFModel.SqlServer/UOrders.EFModel.SqlServer.csproj", "UOrders.EFModel.SqlServer/"]
COPY ["UOrders.EFModel.Mysql/UOrders.EFModel.Mysql.csproj", "UOrders.EFModel.Mysql/"]
RUN dotnet restore "UOrders.Service/UOrders.Service.csproj"
COPY . .
WORKDIR "/src/UOrders.Service"
RUN dotnet build "UOrders.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UOrders.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UOrders.Service.dll"]
