﻿using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OutBoxPattern.Api.Application.Exceptions;
using OutBoxPattern.Api.Domain;
using OutBoxPattern.Api.Domain.Abstractions;
using OutBoxPattern.Api.Infrastructure.Outbox;

namespace OutBoxPattern.Api.Infrastructure.Data;

public class UserDbContext : DbContext, IUnitOfWork
{
  public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
  {
  }

  public DbSet<User> Users => Set<User>();
  public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      AddDomainEventsAsOutboxMessages();

      var result = await base.SaveChangesAsync(cancellationToken);

      return result;
    }
    catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
    {
      throw new ConcurrencyException("Concurrency exception occurred.", ex);
    }
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);

    base.OnModelCreating(modelBuilder);
  }

  private void AddDomainEventsAsOutboxMessages()
  {
    var outboxMessages = ChangeTracker
      .Entries<Entity>()
      .Select(entry => entry.Entity)
      .SelectMany(entity =>
      {
        var domainEvents = entity.GetDomainEvents();

        entity.ClearDomainEvents();

        return domainEvents;
      })
      .Select(domainEvent => new OutboxMessage(
        Guid.NewGuid(),
        domainEvent.GetType().AssemblyQualifiedName!,
        JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
      ))
      .ToList();

    AddRange(outboxMessages);
  }

  private static bool IsUniqueConstraintViolation(DbUpdateException ex)
  {
    if (ex.InnerException is PostgresException pgEx) return pgEx.SqlState == PostgresErrorCodes.UniqueViolation;
    return false;
  }
}