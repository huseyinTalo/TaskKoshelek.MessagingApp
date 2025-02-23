FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaskKoshelek.MessagingApp.API/TaskKoshelek.MessagingApp.API.csproj", "TaskKoshelek.MessagingApp.API/"]
COPY ["TaskKoshelek.MessagingApp.Core/TaskKoshelek.MessagingApp.Core.csproj", "TaskKoshelek.MessagingApp.Core/"]
COPY ["TaskKoshelek.MessagingApp.DAL/TaskKoshelek.MessagingApp.DAL.csproj", "TaskKoshelek.MessagingApp.DAL/"]
RUN dotnet restore "TaskKoshelek.MessagingApp.API/TaskKoshelek.MessagingApp.API.csproj"
COPY . .
RUN dotnet build "TaskKoshelek.MessagingApp.API/TaskKoshelek.MessagingApp.API.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "TaskKoshelek.MessagingApp.API/TaskKoshelek.MessagingApp.API.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_HTTP_PORTS=9098
EXPOSE 9098
ENTRYPOINT ["dotnet", "TaskKoshelek.MessagingApp.API.dll"]