#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["UOrders.PrintService/UOrders.PrintService.csproj", "UOrders.PrintService/"]
COPY ["UOrders.DTOModel/UOrders.DTOModel.csproj", "UOrders.DTOModel/"]
RUN dotnet restore "UOrders.PrintService/UOrders.PrintService.csproj"
COPY . .
WORKDIR "/src/UOrders.PrintService"
RUN dotnet build "UOrders.PrintService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UOrders.PrintService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UOrders.PrintService.dll"]