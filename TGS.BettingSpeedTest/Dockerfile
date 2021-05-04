FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TGS.BettingSpeedTest.csproj", "./"]
RUN dotnet restore "TGS.BettingSpeedTest.csproj"
COPY . .
# change the configuration (-c) from QA to LOCAL if you want to test with local OP/GP API.
RUN dotnet publish "TGS.BettingSpeedTest.csproj" -c STAGING -o /publish
WORKDIR /publish
ENTRYPOINT ["dotnet", "TGS.BettingSpeedTest.dll"]