FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY courseWork/courseWork.csproj courseWork/
COPY courseWork.BLL/courseWork.BLL.csproj courseWork.BLL/
COPY courseWork.DAL/courseWork.DAL.csproj courseWork.DAL/

RUN dotnet restore courseWork/courseWork.csproj

COPY . .
WORKDIR /src/courseWork
RUN dotnet build courseWork.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish courseWork.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "courseWork.dll"]

