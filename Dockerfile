#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["my-diary.Api/my-diary.Api.csproj", "my-diary.Api/"]
COPY ["my-diary.Api.Tests/my-diary.Api.Tests.csproj", "my-diary.Api.Tests/"]
RUN dotnet restore "my-diary.Api/my-diary.Api.csproj"
COPY . .
WORKDIR "/src/my-diary.Api"
RUN dotnet build "my-diary.Api.csproj" -c Release -o /app/build

#Testing
WORKDIR "/src/my-diary.Api.Tests"
RUN dotnet test

#Publish
FROM build AS publish
WORKDIR "/src/my-diary.Api"
RUN dotnet publish "my-diary.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "my-diary.Api.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet my-diary.Api.dll