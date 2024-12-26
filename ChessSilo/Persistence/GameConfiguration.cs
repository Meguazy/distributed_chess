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

            builder.HasIndex(ob => ob.StartedOn);
        }
    }
}