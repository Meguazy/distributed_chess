# Distributed Chess Website

## Overview

This project showcases a distributed web application designed for playing chess in a modern, scalable environment. The application employs Microsoft Orleans for its actor-based architecture, Redis for efficient caching, Azure SQL Edge for database management, and HashiCorp Vault for secure storage of credentials. By combining these technologies, the system delivers a seamless, real-time chess-playing experience while demonstrating core distributed systems principles such as asynchronous communication, state management, and transactional integrity.

## Core Features

The application is built around a modular design with several essential features:

### Actor-Based Architecture

The backbone of the application lies in its actor model, implemented using Microsoft Orleans. Grains, representing individual chess players and games, encapsulate both state and behavior. This architecture enables high scalability and fault isolation, as each game grain coordinates the state of two player grains independently.

### Interactive Frontend

The frontend provides an intuitive interface for chess gameplay, offering:
- **Game Creation:** Users can initiate new chess sessions.
- **Game Participation:** Players can join active games and engage with the chessboard.
- **Real-Time Gameplay:** Regular updates every 40ms ensure a smooth and responsive experience.

### High-Performance Caching with Redis

Redis serves as the caching layer, enhancing the performance of real-time updates:
- **Quick Response Times:** Cached chessboard states minimize latency.
- **Efficient Resource Management:** Cache entries expire after 5 minutes, preventing unnecessary resource usage.
- **Consistency:** The cache is invalidated automatically after each move, ensuring accurate updates.

### Secure Database Management

The system employs Azure SQL Edge to store persistent game data, such as:
- Player details (e.g., White and Black).
- Game metadata (e.g., start/end timestamps, status, and winner).

To secure database credentials, **HashiCorp Vault** integrates seamlessly, mitigating the risk of exposure.

### Distributed Systems Principles

The project adheres to several foundational distributed systems concepts:
- **Asynchronous Communication:** Enables scalable, non-blocking operations.
- **Transactional Integrity:** Ensures data consistency during state updates and database operations.

## Technologies

The application incorporates a range of cutting-edge technologies:
- **Backend:** Microsoft Orleans, Redis, Azure SQL Edge, HashiCorp Vault.
- **Frontend:** A lightweight UI for interacting with the chessboard.
- **Caching:** Redis for low-latency data retrieval.
- **Database:** Azure SQL Edge for reliable storage of game metadata.

## System Architecture

The system architecture reflects a distributed, modular design. Below is a diagram illustrating the high-level structure:

![System Architecture](./path/to/architecture-diagram.png) <!-- Replace with actual path -->

## Workflow

### Starting a New Game
The frontend initializes a game session, triggering the backend to create grains for the game and players. Game metadata is stored in the database for persistence.

### Joining an Active Game
Players can view active games and join an ongoing session. Once connected, they interact with the chessboard through the frontend.

### Real-Time Gameplay
The frontend polls the backend every 40ms for updates. Cached states are retrieved from Redis to ensure rapid responses, with fallback mechanisms to fetch fresh states from the backend if needed.

### Making Moves
When a player makes a move, the backend validates it and updates the game state. This triggers cache invalidation and ensures the latest state is available. Metadata updates, such as game progress and results, are committed to the database in a transactional manner.

## Database Schema

The database is structured to store essential game information effectively:

| Field          | Description                       |
|----------------|-----------------------------------|
| `GameID`       | Unique identifier for the game.   |
| `Description`  | Brief description of the game.    |
| `PlayerWhite`  | Name of the white player.         |
| `PlayerBlack`  | Name of the black player.         |
| `StartedOn`    | Timestamp for game initiation.    |
| `EndedOn`      | Timestamp for game conclusion.    |
| `Winner`       | Name of the winning player.       |
| `Status`       | Current game status.              |

![Database Schema](./path/to/database-schema.png) <!-- Replace with actual path -->

## Getting Started

### Prerequisites
To run the project, ensure the following tools and services are installed:
- .NET 6 SDK
- Redis Server
- Azure SQL Edge
- HashiCorp Vault

### Steps to Run the Project

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd <repository-folder>
   ```

2. Start the backend services:
   ```bash
   dotnet run --project <backend-project-path>
   ```

3. Launch the frontend application:
   ```bash
   <frontend-command>
   ```

4. Verify that Redis and the database are running and properly configured.

## Example Screenshots

### Main Page
![Main Page](./path/to/main-page-screenshot.png) <!-- Replace with actual path -->

### Game Page
![Game Page](./path/to/game-page-screenshot.png) <!-- Replace with actual path -->

## Contributions

Contributions are highly encouraged. If you have ideas for improvements or new features, please submit an issue or a pull request.

## License

This project is licensed under the MIT License. For details, see the `LICENSE` file included in the repository.
