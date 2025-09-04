# Используем SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем .csproj и восстанавливаем зависимости (используем кэш слоёв)
COPY AspNetCoreGitActions/*.csproj ./AspNetCoreGitActions/
RUN dotnet restore "AspNetCoreGitActions/AspNetCoreGitActions.csproj"

# Копируем всё остальное
COPY . .

# Публикуем в папку /app/publish
RUN dotnet publish "AspNetCoreGitActions/AspNetCoreGitActions.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Финальный образ — только runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копируем опубликованные файлы из этапа сборки
COPY --from=build /app/publish .

# Устанавливаем переменную среды (опционально)
ENV ASPNETCORE_URLS=http://+:80

# Открываем порты
EXPOSE 80/tcp
EXPOSE 443/tcp

# Запускаем приложение
ENTRYPOINT ["dotnet", "AspNetCoreGitActions.dll"]
