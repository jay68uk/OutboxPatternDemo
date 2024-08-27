using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutBoxPattern.Api.Domain;

namespace OutBoxPattern.Api.Infrastructure.Configuration;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("users");

    builder.HasKey(user => user.Id);

    builder.Property(user => user.FirstName)
      .HasMaxLength(200);

    builder.Property(user => user.LastName)
      .HasMaxLength(200);

    builder.Property(user => user.Email)
      .HasMaxLength(400);

    builder.HasIndex(user => user.Email).IsUnique();

    builder.HasIndex(user => user.Id).IsUnique();
  }
}