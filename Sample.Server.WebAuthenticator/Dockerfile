FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /Sample.Server.WebAuthenticator

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /Sample.Server.WebAuthenticator
COPY --from=build-env Sample.Server.WebAuthenticator/out .

CMD dotnet Sample.Server.WebAuthenticator.dll --urls "http://*:$PORT"
