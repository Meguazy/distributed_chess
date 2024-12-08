#!/bin/bash

# Create the root directory
mkdir ChessGame
cd ChessGame

# Initialize a solution file
dotnet new sln -n ChessGame

# Create and configure each subproject

## ChessGame.Api
dotnet new webapi -n ChessGame.Api --no-https
dotnet sln add ChessGame.Api/ChessGame.Api.csproj

## ChessGame.Grains
dotnet new classlib -n ChessGame.Grains
dotnet sln add ChessGame.Grains/ChessGame.Grains.csproj

## ChessGame.SiloHost
dotnet new console -n ChessGame.SiloHost
dotnet sln add ChessGame.SiloHost/ChessGame.SiloHost.csproj

## ChessGame.Shared
dotnet new classlib -n ChessGame.Shared
dotnet sln add ChessGame.Shared/ChessGame.Shared.csproj

## ChessGame.Tests
dotnet new xunit -n ChessGame.Tests
dotnet sln add ChessGame.Tests/ChessGame.Tests.csproj

# Final message
echo "Directory structure and projects created successfully!"
