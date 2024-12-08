#!/bin/bash
cd ChessGame
# Ensure we are in the root directory of the solution
cd "$(dirname "$0")/ChessGame"

# Step 1: Restore all dependencies
echo "Restoring dependencies for all projects..."
dotnet restore

# Step 2: Build all projects in the solution
echo "Building all projects..."
dotnet build

# Step 3: Start the Orleans Silo (ChessGame.SiloHost)
echo "Starting the Orleans Silo (ChessGame.SiloHost)..."
# Run ChessGame.SiloHost in the background so that the API can run
(cd ChessGame.SiloHost && dotnet run) &

# Step 4: Start the API (ChessGame.Api)
echo "Starting the API (ChessGame.Api)..."
# Run the ChessGame.Api project
(cd ChessGame.Api && dotnet run)

# Final message
echo "Chess Game application has started!"
