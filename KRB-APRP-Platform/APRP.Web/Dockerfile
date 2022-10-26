#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


#Get base sdk image from microsoft
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

#Copy the CSproj file and restore and dependencies (via NUGET)
COPY ["APRP.Web/APRP.Web.csproj", "APRP.Web/"]
RUN dotnet restore "APRP.Web/APRP.Web.csproj"

# Copy the project files to build our release
COPY . .
WORKDIR "/src/APRP.Web"
RUN dotnet build "APRP.Web.csproj" -c Release -o /app/build

#Publish our build in readiness for deployment
FROM build AS publish
RUN dotnet publish "APRP.Web.csproj" -c Release -o /app/publish

#Copy our publish to runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APRP.Web.dll"]