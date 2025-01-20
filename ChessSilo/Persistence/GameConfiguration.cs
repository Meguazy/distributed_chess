using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChessSilo.Persistence
{
    internal sealed class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable(name: "Games");

            builder.HasKey(ob => ob.GameId);

            builder.Property(ob => ob.GameId).HasColumnName("GameId");
            builder.Property(ob => ob.Description).HasColumnName("Description");
            builder.Property(ob => ob.StartedOn).HasColumnName("StartedOn");
            builder.Property(ob => ob.EndedOn).HasColumnName("EndedOn");
            builder.Property(ob => ob.Winner).HasColumnName("Winner");
            builder.Property(ob => ob.PlayerBlack).HasColumnName("PlayerBlack");
            builder.Property(ob => ob.PlayerWhite).HasColumnName("PlayerWhite");

            builder.HasIndex(ob => ob.StartedOn);
        }
    }
}