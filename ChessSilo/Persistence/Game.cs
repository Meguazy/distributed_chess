using System;

namespace ChessSilo.Persistence
{
    public class Game
    {
        public Guid GameId { get; set; }
        public string Description { get; set; } = default!;
        public DateTime StartedOn { get; set; } = DateTime.UtcNow;
        public DateTime? EndedOn { get; set; }
        public string Status { get; set; } = "Active";
        public string? Winner { get; set; }
        public string PlayerBlack { get; set; } = "";
        public string PlayerWhite { get; set; } = "";
    }
}