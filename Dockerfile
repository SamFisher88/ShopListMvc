# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ShopListMvc/*.csproj ./ShopListMvc/
RUN dotnet restore
# install dotnet-ef
RUN dotnet tool install --global dotnet-ef
# apply migrations
RUN dotnet ef database update

# copy everything else and build app
COPY ShopListMvc/. ./ShopListMvc/
WORKDIR /source/ShopListMvc
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ShopListMvc.dll"]