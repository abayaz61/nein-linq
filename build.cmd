@echo off

if [%appveyor_repo_branch%]==[release] (
  set build_options=--configuration Release
) else if defined appveyor_build_number (
  set build_options=--configuration Release --version-suffix ci%appveyor_build_number%
) else (
  set build_options=--configuration Release --version-suffix yolo
)

dotnet restore test\NeinLinq.Tests\NeinLinq.Tests.csproj || goto :eof
dotnet build test\NeinLinq.Tests\NeinLinq.Tests.csproj %build_options% || goto :eof
dotnet build test\NeinLinq.Tests\NeinLinq.Tests.csproj %build_options:Release=Debug% || goto :eof

dotnet pack src\NeinLinq\NeinLinq.csproj --include-symbols %build_options% || goto :eof
dotnet pack src\NeinLinq.Queryable\NeinLinq.Queryable.csproj --include-symbols %build_options% || goto :eof
dotnet pack src\NeinLinq.Interactive\NeinLinq.Interactive.csproj --include-symbols %build_options% || goto :eof
dotnet pack src\NeinLinq.EntityFramework\NeinLinq.EntityFramework.csproj --include-symbols %build_options% || goto :eof
dotnet pack src\NeinLinq.EntityFrameworkCore\NeinLinq.EntityFrameworkCore.csproj --include-symbols %build_options% || goto :eof
