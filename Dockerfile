FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["YOLO.CaseStudy.API/YOLO.CaseStudy.API.csproj", "YOLO.CaseStudy.API/"]
COPY ["YOLO.CaseStudy.Business/YOLO.CaseStudy.Business.csproj", "YOLO.CaseStudy.Business/"]
COPY ["YOLO.CaseStudy.Entities/YOLO.CaseStudy.Entities.csproj", "YOLO.CaseStudy.Entities/"]
RUN dotnet restore "YOLO.CaseStudy.API/YOLO.CaseStudy.API.csproj"
COPY . .
WORKDIR "/src/YOLO.CaseStudy.API"
RUN dotnet build "YOLO.CaseStudy.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YOLO.CaseStudy.API.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT="Production"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YOLO.CaseStudy.API.dll"]