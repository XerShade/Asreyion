# Use the SDK image to build and publish.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish

# Set the build configuration.
ARG BUILD_CONFIGURATION=Release

# Set the working directory and build the application.
WORKDIR /src
COPY ["applications/Asreyion.Server/Asreyion.Server.csproj", "applications/Asreyion.Server/"]
RUN dotnet restore "./applications/Asreyion.Server/Asreyion.Server.csproj"
COPY . .
WORKDIR "/src/applications/Asreyion.Server"
RUN dotnet publish "./Asreyion.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image: Set up the runtime image and generate certificate if missing.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Set the working directory.
WORKDIR /app

# Expose HTTP and HTTPS ports.
EXPOSE ${ASPNETCORE_HTTP_PORTS}
EXPOSE ${ASPNETCORE_HTTPS_PORTS}

# Copy the app from the publish step.
COPY --from=publish /app/publish .

# Start the application.
ENTRYPOINT ["dotnet", "Asreyion.Server.dll"]
