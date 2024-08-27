using System.Data;

namespace OutBoxPattern.Api.Application.Abstractions;

public interface ISqlConnectionFactory
{
  IDbConnection CreateConnection();
}