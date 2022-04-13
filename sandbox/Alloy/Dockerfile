#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["sandbox/Alloy/AlloyMvcTemplates.csproj", "sandbox/Alloy/"]
COPY ["src/Geta.Optimizely.GenericLinks/Geta.Optimizely.GenericLinks.csproj", "src/Geta.Optimizely.GenericLinks/"]
RUN dotnet restore "sandbox/Alloy/AlloyMvcTemplates.csproj"
COPY . .
WORKDIR "/src/sandbox/Alloy"
RUN dotnet build "AlloyMvcTemplates.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AlloyMvcTemplates.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlloyMvcTemplates.dll"]