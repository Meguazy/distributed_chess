# Distributed Chess: A Cloud-Native Chess Platform

Distributed Chess is a web-based chess application that demonstrates the principles of distributed systems architecture through a practical, real-world implementation. The project showcases how modern cloud-native technologies can be orchestrated to create a low-latency, scalable gaming platform capable of managing multiple concurrent chess matches while maintaining state consistency and security.

## Overview

At its core, this application allows users to play chess through a web interface, but beneath this simple premise lies a sophisticated distributed architecture. The system manages game state across multiple services, balances the competing demands of consistency and performance, and demonstrates secure handling of sensitive credentials in a containerized environment. The architecture brings together several enterprise-grade technologies: Microsoft Orleans for actor-based distributed computing, Redis for high-performance caching, Azure SQL Edge for persistent storage, and HashiCorp Vault for secrets management.

The choice of technologies reflects real-world patterns used in production systems where state management, scalability, and security are paramount concerns. By implementing a chess platform, the project provides a tangible demonstration of how these technologies integrate to solve common distributed systems challenges such as state synchronization, caching strategies, and secure credential management.

## System Architecture

![System Architecture](schema.png)

The application follows a layered architecture where each component serves a specific purpose in the overall system. The frontend presents a simple web interface that communicates with the backend through RESTful HTTP endpoints. This client-side application polls the server every 40 milliseconds to retrieve the current board state, ensuring players see near-real-time updates as moves are made. While this polling approach is straightforward to implement and understand, it also highlights the tradeoffs inherent in distributed system design—specifically the balance between simplicity and efficiency.

The backend, implemented as the ChessSilo project, leverages Microsoft Orleans to model the domain as a collection of distributed actors called grains. Each game is represented by a GameGrain that encapsulates all the logic and state for that particular match. Similarly, PlayerGrains represent individual players and their associated state. This actor-based model provides natural isolation between games, allowing the system to scale horizontally by distributing grains across multiple servers as load increases. Orleans handles the complexity of grain activation, deactivation, and location transparency, allowing the application code to focus on business logic rather than distributed systems plumbing.

State management in the system operates on two levels. Redis serves as a high-speed cache for board states, providing sub-millisecond access times for frequently requested game positions. When a client requests the current board state, the system first checks Redis. If the state is cached, it's returned immediately. Otherwise, the system retrieves the state from the GameGrain, serves it to the client, and stores it in Redis with a five-minute expiration time. When a player makes a move, the cache entry for that game is explicitly invalidated, ensuring the next request fetches the updated state. This write-through cache pattern balances the need for fast reads with the requirement for consistency after state changes.

Persistent storage is handled by Azure SQL Edge, a containerized SQL Server variant designed for edge computing scenarios. The database stores game metadata including unique identifiers, player names, start and end timestamps, game status, and winner information. This persistent layer ensures that game history survives service restarts and provides an audit trail of completed matches. The separation between cached board states in Redis and metadata in SQL demonstrates the principle of polyglot persistence—using different storage technologies optimized for their specific access patterns.

Security is addressed through integration with HashiCorp Vault, which manages database credentials as secrets. Rather than hardcoding connection strings or storing passwords in configuration files, the application retrieves credentials dynamically from Vault at startup. This approach follows security best practices by centralizing secrets management, enabling credential rotation without code changes, and ensuring sensitive information never appears in source control or deployment artifacts.

## Implementation Details

The ChessSilo backend is organized into several key areas that reflect the concerns of a well-structured distributed application. The Grains directory contains the Orleans actor implementations, including GameGrain which orchestrates game lifecycle and move validation, and PlayerGrain which manages player identity and session state. These grains communicate through well-defined interfaces that specify their contracts, promoting loose coupling and testability.

Game logic resides in specialized components that handle move validation, board state representation, and piece behavior. The MoveValidator ensures that attempted moves conform to chess rules, checking factors like whether the piece exists at the starting position, whether the destination is valid, and whether the move would capture the player's own piece. The Chessboard class maintains the internal representation of piece positions and provides methods to apply moves and serialize the current state for transmission to clients.

The Controller layer exposes HTTP endpoints that external clients use to interact with the system. These endpoints handle game creation, move submission, retrieval of active games, and board state queries. Each endpoint follows a similar pattern: validate the request, locate the appropriate grain using Orleans' grain factory, invoke grain methods to perform the requested operation, and return an appropriate HTTP response. Error handling and logging are woven throughout to aid in debugging and monitoring.

Persistence operations are managed through Entity Framework Core, which provides an object-relational mapping between C# domain objects and SQL tables. The GamesContext defines the database schema through code-first migrations, ensuring that database structure remains synchronized with the application model as it evolves. Configuration classes specify table names, column mappings, indexes, and relationships, giving precise control over the generated database schema.

The integration with Redis and Vault occurs during application startup in the Program.cs file. The application establishes connections to these services, registers them in the dependency injection container, and makes them available throughout the application lifecycle. The use of dependency injection promotes testability and allows for easy substitution of alternative implementations during testing or when deployment environments differ.

## Getting Started

### Prerequisites

Ensure you have the following tools installed on your system:

- [.NET 8 SDK](https://dotnet.microsoft.com/download) - Required to build and run the application
- [Docker](https://docs.docker.com/get-docker/) - Used to run infrastructure services (Redis, SQL Server, Vault)
- [Vault CLI](https://developer.hashicorp.com/vault/downloads) - Required for configuring Vault secrets

### Setup Instructions

**1. Clone the Repository**

Begin by cloning the repository to your local machine:

```bash
git clone https://github.com/Meguazy/distributed_chess.git
cd distributed_chess
```

**2. Configure Environment Variables**

Create a `.env` file in the `ChessSilo` directory with the following content:

```bash
SA_PASSWORD=YourStrongPassword123!
ACCEPT_EULA=Y
VAULT_DEV_ROOT_TOKEN_ID=myroot
```

Replace `YourStrongPassword123!` with a secure password for SQL Server and adjust the Vault token as needed.

**3. Start Infrastructure Services**

Launch the required services using Docker Compose:

```bash
cd ChessSilo
docker-compose up -d
```

This command starts Redis, Azure SQL Edge, and HashiCorp Vault in detached mode. Verify the services are running:

```bash
docker-compose ps
```

**4. Configure Vault Secrets**

Set up your environment to interact with Vault:

```bash
export VAULT_ADDR=http://127.0.0.1:8200
export VAULT_TOKEN=myroot
```

Enable the KV version 2 secrets engine at the `database` path:

```bash
vault secrets enable -path=database kv-v2
```

Store the database credentials in Vault:

```bash
vault kv put database/configs username="sa" password="YourStrongPassword123!"
```

Verify the secrets were stored correctly:

```bash
vault kv get database/configs
```

**5. Build and Run the Application**

Restore dependencies and build the solution:

```bash
dotnet restore
dotnet build
```

Run the application:

```bash
dotnet run --project ChessSilo
```

The application will start on `http://localhost:7164` and the Orleans Dashboard will be available at `http://localhost:8080`.

**6. Access the Application**

Open your web browser and navigate to:

```
http://localhost:7164
```

You should see the main page listing active chess games. You can create a new game using the REST API or by interacting with the provided endpoints.

### Testing the Application

You can test the application using the provided REST API endpoints. A `Test.rest` file is included in the ChessSilo directory with example requests. If you're using Visual Studio Code with the REST Client extension, you can execute these requests directly.

Example: Starting a new game:

```bash
curl -X POST http://localhost:7164/ChessGame/start \
  -H "Content-Type: application/json" \
  -d '{
    "PlayerWhiteName": "Alice",
    "PlayerBlackName": "Bob"
  }'
```

Example: Making a move:

```bash
curl -X POST http://localhost:7164/ChessGame/move \
  -H "Content-Type: application/json" \
  -d '{
    "GameId": "your-game-id-here",
    "Move": "P-e2-e4",
    "PlayerName": "Alice"
  }'
```

## Learning Outcomes and Design Decisions

This project illustrates several important concepts in distributed systems development. The actor model provided by Orleans demonstrates how complex state can be partitioned and managed through isolated, single-threaded actors that communicate via asynchronous message passing. This model avoids many concurrency pitfalls by ensuring each grain processes one message at a time, eliminating the need for explicit locking while still enabling parallelism across grains.

The caching strategy exemplifies a common pattern where frequently accessed data is kept in a fast storage tier while persistent storage maintains the source of truth. The explicit cache invalidation on writes ensures consistency, though at the cost of some cache efficiency. More sophisticated strategies could employ cache warming, smarter expiration policies, or even event-driven invalidation patterns.

Security through Vault integration demonstrates that credential management deserves first-class treatment in application design. By externalizing secrets to a dedicated service using the KV version 2 secrets engine, the application gains flexibility in credential rotation, centralized audit logging, version history for secrets, and reduced risk from compromised source code or configuration files. The use of KV v2 specifically provides additional features like secret versioning and the ability to soft-delete and undelete secrets.

The polling-based frontend, while simple to implement and understand, represents a tradeoff that real production systems would likely handle differently. WebSockets or server-sent events would provide more efficient real-time updates, reducing network overhead and improving responsiveness. The current implementation serves as a baseline that could be enhanced to demonstrate more advanced communication patterns.

## License

Distributed Chess is licensed under the [Apache-2.0 License](LICENSE).
