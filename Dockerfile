# Utiliza una imagen base de .NET 7
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Establece el directorio de trabajo dentro del contenedor
WORKDIR /app

# Copia los archivos de tu aplicación al directorio de trabajo en el contenedor
COPY ./*.csproj ./

# Restaura las dependencias y compila el proyecto
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Crea una imagen de producción más pequeña
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Establece el directorio de trabajo en la imagen de producción
WORKDIR /app

# Copia los archivos publicados desde la etapa de compilación
COPY --from=build /app/out .

# Copia el archivo Dockerfile al directorio de trabajo en la imagen de producción
COPY Dockerfile .

# Expone el puerto en el que tu aplicación está escuchando
EXPOSE 80

#set entry point 
ENTRYPOINT ["dotnet", "MyBackend_MongoDB_CSharp.dll"]


