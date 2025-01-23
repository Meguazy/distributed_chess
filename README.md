# Distributed Chess Website

## Overview

This project demonstrates a distributed web application designed for playing chess, leveraging modern technologies for scalability and efficiency. It employs Microsoft Orleans for its actor-based architecture, Redis for caching, Azure SQL Edge for database management, and HashiCorp Vault for credential security. Together, these technologies enable real-time chess gameplay while adhering to key distributed systems principles like asynchronous communication, state management, and transactional integrity.

## Core Features

The application is architected around Microsoft Orleans’ actor model. Grains encapsulate the state and behavior of individual chess players and games, ensuring scalability and fault isolation. The frontend provides an intuitive interface for users to create games, join ongoing matches, and play in real-time with updates delivered every 40ms for seamless interaction. Redis ensures quick response times by caching chessboard states, which are automatically invalidated after each move to maintain consistency. Persistent game data, including player details, metadata, and results, is securely stored in Azure SQL Edge, with credentials managed by HashiCorp Vault to enhance security. 

## Technologies

The backend integrates Microsoft Orleans, Redis, Azure SQL Edge, and HashiCorp Vault, while the frontend facilitates user interaction. Redis improves the performance of real-time gameplay by enabling low-latency data retrieval. Azure SQL Edge handles the reliable storage of game metadata. These technologies work cohesively to deliver an efficient and secure chess-playing platform.

## System Architecture

The distributed design of the system ensures modularity and scalability. Grains in Microsoft Orleans represent individual players and games, communicating asynchronously to maintain state and behavior. Redis acts as an intermediary for caching frequently accessed states, and Azure SQL Edge stores game information persistently. HashiCorp Vault secures sensitive data, such as database credentials. A lightweight frontend connects players to the backend, ensuring a user-friendly experience.

## Workflow

The chess-playing experience is streamlined into clear workflows. When a user starts a new game, the frontend triggers the backend to create grains for the game and players, storing metadata in the database for persistence. Players can join active games and interact with the chessboard via the frontend, which polls the backend for updates every 40ms. Moves are validated by the backend, updating the game state, invalidating the cache, and committing changes to the database within a transaction to ensure consistency. 

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

## Getting Started

### Prerequisites
To run the project, ensure the following tools and services are installed:
- .NET 6 SDK
- Docker

### Steps to Run the Project

1. Clone the repository:
   ```bash
   git clone https://github.com/Meguazy/distributed_chess.git
   cd ChessSilo/
   ```

2. Start the Docker service:
   ```bash
   docker-compose up -d
   ```
   
3. Install the dependencies from the .csproj file:
   ```bash
   dotnet restore
   ```

4. Build the app:
   ```bash
   dotnet build
   ```

5. Start the app:
   ```bash
   dotnet run
   ```

6. You can open the front end by navigatin to:
   ```
   http://localhost:7164
   ```
   You will be automatically redirected to 

## Contributions

Contributions are highly encouraged. If you have ideas for improvements or new features, please submit an issue or a pull request.

## License

This project is licensed under the MIT License. For details, see the `LICENSE` file included in the repository.
