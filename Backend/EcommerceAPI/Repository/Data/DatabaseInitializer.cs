using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Repository.Data
{
    /// <summary>
    /// Initializes the database schema with required migrations.
    /// This runs on application startup to ensure all tables and columns exist.
    /// </summary>
    public class DatabaseInitializer
    {
        private readonly IConfiguration _configuration;

        public DatabaseInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Runs database migrations to ensure schema is up to date.
        /// </summary>
        public void Initialize()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    AddStateColumnToOrders(connection);

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
            }
        }

        private void AddStateColumnToOrders(SqlConnection connection)
        {
            string query = @"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                               WHERE TABLE_NAME = 'Orders' AND COLUMN_NAME = 'State')
                BEGIN
                    ALTER TABLE Orders ADD State NVARCHAR(100) NULL;
                END";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is not configured");
        }
    }
}
