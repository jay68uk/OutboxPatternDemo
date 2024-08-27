using Microsoft.EntityFrameworkCore;
using OutBoxPattern.Api.Infrastructure.Data;

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

    return builder;
  }

  public static IServiceCollection AddApplication(this IServiceCollection builder)
  {
    builder.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly); });

    return builder;
  }
}