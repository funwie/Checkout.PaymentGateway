FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["../src/PaymentGateway/PaymentGateway.csproj", "src/PaymentGateway/"]
RUN dotnet restore "src/PaymentGateway/PaymentGateway.csproj"
COPY . .
WORKDIR "/src/src/PaymentGateway"
RUN dotnet build "PaymentGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentGateway.dll"]