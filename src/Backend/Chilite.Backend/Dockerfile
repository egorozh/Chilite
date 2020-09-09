#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Backend/Chilite.Backend/Chilite.Backend.csproj", "Backend/Chilite.Backend/"]
COPY ["Frontend/Web/Chilite.Web/Chilite.Web.csproj", "Frontend/Web/Chilite.Web/"]
COPY ["Shared/Chilite.Protos/Chilite.Protos.csproj", "Shared/Chilite.Protos/"]
COPY ["Backend/Chilite.Database/Chilite.Database.csproj", "Backend/Chilite.Database/"]
RUN dotnet restore "Backend/Chilite.Backend/Chilite.Backend.csproj"
COPY . .
WORKDIR "/src/Backend/Chilite.Backend"
RUN dotnet build "Chilite.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Chilite.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chilite.Backend.dll"]