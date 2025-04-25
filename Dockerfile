ARG builder_image=mcr.microsoft.com/dotnet/sdk:9.0
FROM ${builder_image} AS build

WORKDIR /src
# The below allows layer caching for the restore.
COPY RazorPageLeifExample/RazorPageLeifExample.csproj .
RUN dotnet restore
COPY RazorPageLeifExample ./
RUN dotnet publish $csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
ENV ASPNETCORE_URLS=http://*:3001
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 3001
ENTRYPOINT ["dotnet", "RazorPageLeifExample.dll"]
