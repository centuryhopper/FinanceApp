# Stage 1: Build Client
FROM node:20 AS Client-build
WORKDIR /app/Client
COPY Client/package*.json ./
RUN npm install
COPY Client/ .
RUN npm run build

# Stage 2: Build Server
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Server/*.csproj ./Server/
RUN dotnet restore ./Server
COPY . .
# Copy the built Vue app into wwwroot of Server before publish
RUN cp -r /app/Client/dist/* ./Server/wwwroot/
WORKDIR /src/Server
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Server.dll"]
