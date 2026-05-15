using Npgsql;

namespace TechExercise.WebApi.Data;

public interface IDbConnectionFactory
{
    NpgsqlConnection CreateConnection();
}
