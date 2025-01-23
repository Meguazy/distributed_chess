# Distributed Chess Website

## Overview
This project is a distributed web application designed to bring the classic game of chess to a modern, scalable, and efficient platform. The system uses Microsoft Orleans to implement an actor-based architecture, alongside technologies like Redis for caching, Azure SQL Edge for database management, and HashiCorp Vault for secure credential storage. The aim is to provide a responsive, real-time chess-playing experience while demonstrating key distributed systems concepts such as asynchronous communication and transactions.

---

## Features

### Actor Model with Microsoft Orleans
The backend architecture revolves around the actor model, implemented using Microsoft Orleans:
- **Player Grains:** Each player and game is represented as a grain, encapsulating their respective state and logic.
- **Game Grains:** Each game grain depends on two player grains to operate seamlessly, ensuring isolation and scalability.

### Frontend
The frontend is simple but effective, designed to:
- **Start a new game:** Create a new game session and initialize the backend grains.
- **Join an existing game:** Connect to an active game session and interact with the chessboard in real-time.
- **Real-time Updates:** The frontend fetches updates every 40ms, ensuring the chessboard reflects the latest moves instantly.

### Redis Cache
To meet the 40ms update interval requirement, Redis is used to cache the chessboard state:
- **Quick Responses:** Cached states reduce latency during frequent requests.
- **Cache Lifetime:** Entries expire after 5 minutes.
- **Cache Invalidation:** The cache is invalidated whenever a new move is made, ensuring data consistency.

### Database with Secure Credential Management
The database, powered by Azure SQL Edge, stores game metadata such as:
- Player names (White and Black).
- Game start and end timestamps.
- Winner information.
- Current game status.

Credentials for the database are securely managed using HashiCorp Vault, ensuring secure connections without exposing sensitive information.

### Distributed System Concepts
This project incorporates key distributed systems principles:
- **Asynchronous Communication:** Enhances system responsiveness and scalability.
- **Transactions:** Ensures consistency and reliability during game state updates.

---

## Technologies Used
- **Backend:** Microsoft Orleans, Redis, Azure SQL Edge, HashiCorp Vault
- **Frontend:** Minimalist UI for game interactions
- **Caching:** Redis for low-latency responses
- **Database:** Azure SQL Edge for persistent storage

---

## System Architecture
Below is a high-level representation of the system's architecture:

![System Architecture](./path/to/architecture-diagram.png) <!-- Replace with actual path -->

---

## Workflow

1. **Starting a Game:**
   - A new game is initiated through the frontend.
   - The backend creates grains for the game and players, storing metadata in the database.

2. **Joining a Game:**
   - The frontend retrieves a list of active games.
   - Players can select a game and join the session.

3. **Real-Time Updates:**
   - The frontend requests chessboard updates every 40ms.
   - Redis Cache provides the state if available; otherwise, the board service fetches and caches the state.

4. **Making a Move:**
   - The backend validates the move and updates the game state.
   - Cache invalidation ensures subsequent updates reflect the latest state.
   - Game metadata is updated in the database.

---

## Database Schema

The database schema includes the following fields:

| Field          | Description                       |
|----------------|-----------------------------------|
| `GameID`       | Primary key for the game.         |
| `Description`  | Brief description of the game.    |
| `PlayerWhite`  | Name of the white player.         |
| `PlayerBlack`  | Name of the black player.         |
| `StartedOn`    | Timestamp for when the game began.|
| `EndedOn`      | Timestamp for when the game ended.|
| `Winner`       | Name of the winning player.       |
| `Status`       | Current status of the game.       |

![Database Schema](./path/to/database-schema.png) <!-- Replace with actual path -->

---

## Key Concepts

### Actor Model
Grains, the core building blocks in Orleans, encapsulate the state and logic for players and games. This approach simplifies scaling and fault isolation.

### Caching
Redis Cache enhances performance by minimizing the time required for board state retrieval, especially crucial for frequent updates.

### Asynchronous Communication
Non-blocking operations ensure that the system remains responsive under heavy load.

### Transactions
Database operations are transactional, maintaining consistency even in distributed environments.

---

## Getting Started

### Prerequisites
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

4. Ensure Redis and the database are running and properly configured.

---

## Example Screenshots

- **Main Page:**
  ![Main Page](./path/to/main-page-screenshot.png) <!-- Replace with actual path -->

- **Game Page:**
  ![Game Page](./path/to/game-page-screenshot.png) <!-- Replace with actual path -->

---

## Contributions
Contributions are highly encouraged! If you'd like to improve the project, please open an issue or submit a pull request.

---

## License
This project is licensed under the MIT License. See the `LICENSE` file for more details.
