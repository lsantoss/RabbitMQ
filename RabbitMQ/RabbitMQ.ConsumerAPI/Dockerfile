FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["RabbitMQ.ConsumerAPI/RabbitMQ.ConsumerAPI.csproj", "RabbitMQ.ConsumerAPI/"]
RUN dotnet restore "RabbitMQ.ConsumerAPI/RabbitMQ.ConsumerAPI.csproj"
COPY . .
WORKDIR "/src/RabbitMQ.ConsumerAPI"
RUN dotnet build "RabbitMQ.ConsumerAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitMQ.ConsumerAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitMQ.ConsumerAPI.dll"]