using Npgsql;

namespace TechExercise.Infrastructure.Data;

public interface IDbConnectionFactory
{
    NpgsqlConnection CreateConnection();
}
