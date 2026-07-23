using Microsoft.Data.SqlClient;

namespace CoreData
{
    public interface IDatabaseConnection
    {
        public SqlConnection GetConnection();
        public string ConnectionString { get; set; }
    }

    public class DatabaseConnection : IDatabaseConnection
    {
        private string? connectionString;

        public string ConnectionString
        {
            get
            {
                return connectionString ?? string.Empty;
            }

            set
            {
                connectionString = value;
            }
        }

        public DatabaseConnection(string _connectionString)
        {
            ConnectionString = _connectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
