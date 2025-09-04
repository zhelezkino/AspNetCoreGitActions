# ���������� SDK ��� ������
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# �������� .csproj � ��������������� ����������� (���������� ��� ����)
COPY AspNetCoreGitActions/*.csproj ./AspNetCoreGitActions/
RUN dotnet restore "AspNetCoreGitActions/AspNetCoreGitActions.csproj"

# �������� �� ���������
COPY . .

# ��������� � ����� /app/publish
RUN dotnet publish "AspNetCoreGitActions/AspNetCoreGitActions.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# ��������� ����� � ������ runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# �������� �������������� ����� �� ����� ������
COPY --from=build /app/publish .

# ������������� ���������� ����� (�����������)
ENV ASPNETCORE_URLS=http://+:80

# ��������� �����
EXPOSE 80/tcp
EXPOSE 443/tcp

# ��������� ����������
ENTRYPOINT ["dotnet", "AspNetCoreGitActions.dll"]
