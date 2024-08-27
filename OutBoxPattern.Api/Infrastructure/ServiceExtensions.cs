using Microsoft.EntityFrameworkCore;
using OutBoxPattern.Api.Application.Abstractions;
using OutBoxPattern.Api.Domain.Abstractions;
using OutBoxPattern.Api.Infrastructure.Data;
using OutBoxPattern.Api.Infrastructure.Outbox;
using Quartz;

namespace OutBoxPattern.Api.Infrastructure;

public static class ServiceExtensions
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection builder, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("UsersDb") ??
                           throw new ArgumentNullException(nameof(configuration));

    builder.AddDbContext<UserDbContext>(options =>
      options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

    builder.AddScoped<UserRepository>();

    builder.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UserDbContext>());

    builder.AddSingleton<ISqlConnectionFactory>(_ =>
      new SqlConnectionFactory(connectionString));

    return builder;
  }

  public static IServiceCollection AddApplication(this IServiceCollection builder)
  {
    builder.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly); });

    return builder;
  }

  public static void AddBackgroundJobs(this IServiceCollection builder, IConfiguration configuration)
  {
    builder.Configure<OutboxOptions>(configuration.GetSection("Outbox"));

    builder.AddQuartz();

    builder.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

    builder.ConfigureOptions<ProcessOutboxMessagesJobSetup>();
  }
}