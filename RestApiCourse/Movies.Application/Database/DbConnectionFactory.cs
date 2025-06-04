using System.Data;

namespace Movies.Application.Database
{
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Creates a new database connection.
        /// </summary>
        /// <returns>A new instance of a database connection.</returns>
        Task<IDbConnection> CreateConnectionAsync();
    }

    public class NpgsqlDbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public NpgsqlDbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new Npgsql.NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            return connection;
        }
    }
}
