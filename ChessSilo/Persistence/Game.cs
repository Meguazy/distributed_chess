using System;

namespace ChessSilo.Persistence
{
    public class Game
    {
        public Guid GameId { get; set; }
        public string Description { get; set; } = default!;
        public DateTime StartedOn { get; set; } = DateTime.UtcNow;
    }
}