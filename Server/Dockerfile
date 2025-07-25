# Use the ASP.NET runtime base image (will also install Node)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Set working directory
WORKDIR /app

# --- Install Node.js ---
RUN apt-get update && apt-get install -y curl gnupg ca-certificates && \
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs && \
    node --version && npm --version

# --- Copy and Build Frontend ---
COPY Client/ ./Client/
WORKDIR /app/Client
RUN npm install && npm run build

# --- Copy and Publish Backend ---
WORKDIR /app
COPY . .
RUN apt-get install -y dotnet-sdk-8.0
RUN dotnet publish ./Server.csproj -c Release -o /app/publish

# --- Final App Setup ---
WORKDIR /app
COPY --from=0 /app/Client/dist ./wwwroot
COPY /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "Server.dll"]
