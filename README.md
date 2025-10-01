# Distributed Chess

Distributed Chess is a **distributed web-based chess platform** built with modern cloud-native technologies.  
It demonstrates how to design, implement, and deploy an application that requires **low-latency state management**, **scalability**, and **secure service integration**.

The project combines:
- [Microsoft Orleans](https://learn.microsoft.com/en-us/dotnet/orleans/) for distributed actor-based programming  
- [Redis](https://redis.io/) for caching and high-performance access to chessboard states  
- [Azure SQL Edge](https://learn.microsoft.com/en-us/azure/azure-sql-edge/) for persistent storage of game metadata  
- [HashiCorp Vault](https://www.vaultproject.io/) for secure storage and retrieval of database credentials  

---

## 🏗 Architecture

The system is composed of several key components:

- **Frontend (Web Client)**  
  A simple web interface where users can play chess against each other.  
  The client polls the backend every **40 ms** to request the current state of the board.  

- **Backend (ChessSilo)**  
  The backend is built using **Orleans Grains**, which model games, players, and chess logic as distributed actors.  
  It exposes endpoints to handle moves, validate legality, update game state, and synchronize player sessions.  

- **Redis Cache**  
  Maintains the chessboard state in-memory for extremely fast retrieval.  
  - States expire after **5 minutes**.  
  - Whenever a new move is made, the cache is invalidated and refreshed.  

- **SQL Database (Azure SQL Edge)**  
  Stores long-lived metadata about games:
  - `GameID`
  - Game description
  - White/Black player names
  - Start/end timestamps
  - Winner
  - Current status  

- **HashiCorp Vault**  
  Protects database credentials.  
  - The backend retrieves secrets dynamically instead of storing them in code/config.  
  - Credentials are injected at runtime from Vault’s `database` secrets path.  

![System Diagram](schema.png)

---

## 📂 Repository Structure

```
distributed_chess/
├── .vscode/              # Development environment settings
├── ChessSilo/            # Main backend project (Orleans-based)
│   ├── Grains/           # Orleans grains implementing game logic
│   ├── Interfaces/       # Grain contracts and service definitions
│   ├── Models/           # Data models (games, players, moves)
│   ├── Program.cs        # Application entrypoint
│   ├── Startup.cs        # Orleans + Redis + Vault + SQL configuration
│   └── ...
├── schema.drawio         # Architecture diagram (editable)
├── schema.png            # Architecture diagram (exported)
├── ChessGame.sln         # .NET solution file
├── README.md             # Project documentation
└── LICENSE               # Apache-2.0 license
```

### 🔑 ChessSilo in detail
The **ChessSilo** project is the heart of the application:
- **Grains**  
  - `GameGrain`: manages the lifecycle of a chess game, its state, and transitions.  
  - `PlayerGrain`: represents a player, their identity, and active games.  
  - `MoveGrain` (if present): processes moves, enforces chess rules, and updates the board.  

- **Interfaces**  
  Define contracts between grains (e.g., how a `GameGrain` communicates with a `PlayerGrain`).  

- **Models**  
  Contain simple C# classes that describe the domain entities (game state, move objects, player details).  

---

## 🚀 Getting Started

### Prerequisites
- [.NET 6 SDK](https://dotnet.microsoft.com/download)  
- [Docker](https://docs.docker.com/get-docker/)  
- [Vault CLI](https://developer.hashicorp.com/vault/downloads)  

### Setup Steps
1. **Clone the repository**
   ```bash
   git clone https://github.com/Meguazy/distributed_chess.git
   cd distributed_chess
   ```

2. **Start services with Docker**
   ```bash
   docker-compose up -d
   ```
   This launches Redis, SQL Edge, and Vault.

3. **Restore and build the .NET solution**
   ```bash
   dotnet restore
   dotnet build
   ```

4. **Configure Vault**
   - Set environment variables:
     ```bash
     export VAULT_TOKEN=<your-root-token>
     export VAULT_ADDR=http://127.0.0.1:8200
     ```
   - Enable the database secrets engine:
     ```bash
     vault secrets enable database
     ```
   - Add SQL credentials:
     ```bash
     vault kv put database/creds db_username="your_user" db_password="your_password"
     ```

5. **Run the application**
   ```bash
   dotnet run --project ChessSilo
   ```

6. **Access the frontend**
   Open [http://localhost:7164](http://localhost:7164) in your browser.

---

## ⚙️ How It Works

1. **Game Creation**: A player starts a new game. Metadata is persisted in SQL Edge.  
2. **State Management**: Each game is managed by a `GameGrain`.  
3. **Move Execution**: Players submit moves → Orleans grain validates → Redis cache updated.  
4. **State Retrieval**: The frontend polls every 40 ms. If a cache hit occurs, the board is served instantly.  
5. **Cache Expiration**: After 5 minutes (or a new move), the cache invalidates and the grain refreshes the data.  

---

## 📖 Future Improvements

- Replace HTTP polling with **WebSockets** or Orleans streaming for real-time push updates.  
- Add authentication & player accounts.  
- Implement matchmaking and ranked play.  
- Deploy using **Kubernetes** with Orleans clustering.  
- Extend Vault integration with dynamic secret rotation.  

---

## 📜 License
Distributed Chess is licensed under the [Apache-2.0 License](LICENSE).
