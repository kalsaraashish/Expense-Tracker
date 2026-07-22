FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project file first so restore can be cached when source files change.
COPY ["Expense_Tracker.csproj", "./"]
RUN dotnet restore "Expense_Tracker.csproj"

# Copy the rest of the application and publish it for the runtime image.
COPY . .
RUN dotnet publish "Expense_Tracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]

