using Dapper;
using Dapper.Contrib.Extensions;

namespace CoreData.Identity
{
    [Table("(Roles)")]
    public class Roles
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }

        public static async Task<List<Roles>> GetAllRolesAsync(IDatabaseConnection databaseConnection)
        {
            using var connection = databaseConnection.GetConnection();
            const string sql = @"SELECT Id, RoleName, UpdatedBy FROM [dbo].[Roles] ORDER BY RoleName";
            var result = await connection.QueryAsync<Roles>(sql);
            return result.ToList();
        }
    }
}
