using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutBoxPattern.Api.Infrastructure.Outbox;

namespace OutBoxPattern.Api.Infrastructure.Configuration;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
  public void Configure(EntityTypeBuilder<OutboxMessage> builder)
  {
    builder.ToTable("outbox_messages");

    builder.HasKey(outboxMessage => outboxMessage.Id);

    builder.Property(outboxMessage => outboxMessage.Payload).HasColumnType("jsonb");
  }
}