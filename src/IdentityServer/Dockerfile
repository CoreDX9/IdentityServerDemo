#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-sac2016 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM microsoft/dotnet:2.1-sdk-nanoserver-sac2016 AS build
WORKDIR /src
COPY ["IdentityServer/IdentityServer.csproj", "IdentityServer/"]
COPY ["DbMigration/DbMigration.csproj", "DbMigration/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Util/Util.csproj", "Util/"]
COPY ["Domain.EntityFrameworkCore/Domain.EntityFrameworkCore.csproj", "Domain.EntityFrameworkCore/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["EntityFrameworkCore.Extensions/EntityFrameworkCore.Extensions.csproj", "EntityFrameworkCore.Extensions/"]
COPY ["Entity/Entity.csproj", "Entity/"]
RUN dotnet restore "IdentityServer/IdentityServer.csproj"
COPY . .
WORKDIR "/src/IdentityServer"
RUN dotnet build "IdentityServer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "IdentityServer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]