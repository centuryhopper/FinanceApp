# Stage 1: Build Vue.js app
FROM node:18 AS frontend-build
WORKDIR /app
COPY Client/ ./Client/
WORKDIR /app/Client
RUN npm install
RUN npm run build
#
# Stage 2: Build .NET backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy full solution including .csproj and everything it needs
COPY . .

# Restore and build
RUN dotnet restore "./Server.csproj"
RUN dotnet build "./Server.csproj" -c Release -o /app/build

# Stage 3: Publish .NET app
FROM build AS publish
RUN dotnet publish "./Server.csproj" -c Release -o /app/publish

# Stage 4: Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy Vue built files into wwwroot
COPY --from=frontend-build /app/Client/dist ./wwwroot

EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Server.dll"]
