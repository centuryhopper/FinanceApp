# Stage 1: Build Vue.js app
FROM node:18 AS frontend-build
WORKDIR /app
COPY Client/ ./Client/
WORKDIR /app/Client
RUN npm install
RUN npm run build

# Stage 2: Build .NET backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Server.csproj", "./"]
RUN dotnet restore "Server.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Server.csproj" -c Release -o /app/build

# Stage 3: Publish .NET backend
FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

# Stage 4: Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=frontend-build /app/Client/dist ./wwwroot
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Server.dll"]
