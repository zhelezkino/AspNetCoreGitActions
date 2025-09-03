// Needed packages:
// Install-Package Microsoft.EntityFrameworkCore.InMemory
// Install-Package Swashbuckle.AspNetCore
// Install-Package Microsoft.AspNetCore.OpenApi -Version 8.0.8

// In Visual Studio добавить тесты:
// File - Add - New Project
// xUnit Test Project (.NET 8)
// AspNetCoreGitActions.Tests
// + ссылку на основное решение
// dotnet add AspNetCoreGitActions.Tests reference AspNetCoreGitActions/AspNetCoreGitActions.csproj

// Create a new repository on the command line
// git init
// git add .
// git commit -m "First commit"
// git branch -M main
// git remote add origin https://github.com/zhelezkino/AspNetCoreGitActions.git
// git push -u origin main

// Push an existing repository from the command line
// git remote add origin https://github.com/zhelezkino/AspNetCoreGitActions.git
// git branch -M main
// git push -u origin main

// In Visual Studio настройка GitHub Actions:
// Новый файл в проекте: .github/workflows/ci.yml
// ProjectList - Add - New Folder - ".github" - "workflows"
// "workflows" - Add - New Item - ci.yml 
