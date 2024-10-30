# Usa la imagen oficial de .NET SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copia el archivo de solución y los proyectos para restaurar dependencias
COPY ssptb.pe.tdlt.storage.service.sln ./
COPY ssptb.pe.tdlt.storage.api/ssptb.pe.tdlt.storage.api.csproj ./ssptb.pe.tdlt.storage.api/
COPY ssptb.pe.tdlt.storage.command/ssptb.pe.tdlt.storage.command.csproj ./ssptb.pe.tdlt.storage.command/
COPY ssptb.pe.tdlt.storage.commandhandler/ssptb.pe.tdlt.storage.commandhandler.csproj ./ssptb.pe.tdlt.storage.commandhandler/
COPY ssptb.pe.tdlt.storage.commandvalidator/ssptb.pe.tdlt.storage.commandvalidator.csproj ./ssptb.pe.tdlt.storage.commandvalidator/
COPY ssptb.pe.tdlt.storage.common/ssptb.pe.tdlt.storage.common.csproj ./ssptb.pe.tdlt.storage.common/
COPY ssptb.pe.tdlt.storage.data/ssptb.pe.tdlt.storage.data.csproj ./ssptb.pe.tdlt.storage.data/
COPY ssptb.pe.tdlt.storage.dto/ssptb.pe.tdlt.storage.dto.csproj ./ssptb.pe.tdlt.storage.dto/
COPY ssptb.pe.tdlt.storage.entities/ssptb.pe.tdlt.storage.entities.csproj ./ssptb.pe.tdlt.storage.entities/
COPY ssptb.pe.tdlt.storage.infraestructure/ssptb.pe.tdlt.storage.infraestructure.csproj ./ssptb.pe.tdlt.storage.infraestructure/
COPY ssptb.pe.tdlt.storage.internalservices/ssptb.pe.tdlt.storage.internalservices.csproj ./ssptb.pe.tdlt.storage.internalservices/
COPY ssptb.pe.tdlt.storage.redis/ssptb.pe.tdlt.storage.redis.csproj ./ssptb.pe.tdlt.storage.redis/
COPY ssptb.pe.tdlt.storage.secretsmanager/ssptb.pe.tdlt.storage.secretsmanager.csproj ./ssptb.pe.tdlt.storage.secretsmanager/

# Restaura las dependencias
RUN dotnet restore

# Copia el código fuente y compílalo en modo Release
COPY . ./
RUN dotnet publish ssptb.pe.tdlt.storage.api/ -c Release -o /app/out

# Usa una imagen de runtime más ligera para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia los archivos publicados desde la fase de construcción
COPY --from=build-env /app/out .

# Copia el archivo de configuración para producción
COPY ssptb.pe.tdlt.storage.api/appsettings.Production.json ./appsettings.Production.json

# Configura el entorno de producción y la URL
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:80

# Expone el puerto 80 para la aplicación
EXPOSE 80

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "ssptb.pe.tdlt.storage.api.dll"]
