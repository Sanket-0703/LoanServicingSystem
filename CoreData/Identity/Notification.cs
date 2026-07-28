using Dapper;
using Dapper.Contrib.Extensions;

namespace CoreData.Identity
{
    [Table("Notifications")]
    public class Notification
    {
        [ExplicitKey]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? UpdatedBy { get; set; }

        public static async Task<List<Notification>> GetAllAsync(
            IDatabaseConnection databaseConnection,
            Guid userId)
        {
            using var connection = databaseConnection.GetConnection();

            const string sql = @"
SELECT *
FROM Notifications
WHERE UserId=@UserId
ORDER BY CreatedAt DESC";

            var result = await connection.QueryAsync<Notification>(
                sql,
                new { UserId = userId });

            return result.ToList();
        }

        public static async Task CreateAsync(
            IDatabaseConnection databaseConnection,
            Notification notification)
        {
            using var connection = databaseConnection.GetConnection();

            await connection.InsertAsync(notification);
        }

        public static async Task MarkAsReadAsync(
            IDatabaseConnection databaseConnection,
            Guid id)
        {
            using var connection = databaseConnection.GetConnection();

            const string sql = @"
UPDATE Notifications
SET IsRead=1
WHERE Id=@Id";

            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public static async Task MarkAllAsReadAsync(
            IDatabaseConnection databaseConnection,
            Guid userId)
        {
            using var connection = databaseConnection.GetConnection();

            const string sql = @"
UPDATE Notifications
SET IsRead=1
WHERE UserId=@UserId
AND IsRead=0";

            await connection.ExecuteAsync(sql, new { UserId = userId });
        }

        public static async Task<int> GetUnreadCountAsync(
            IDatabaseConnection databaseConnection,
            Guid userId)
        {
            using var connection = databaseConnection.GetConnection();

            const string sql = @"
SELECT COUNT(*)
FROM Notifications
WHERE UserId=@UserId
AND IsRead=0";

            return await connection.ExecuteScalarAsync<int>(
                sql,
                new { UserId = userId });
        }
    }
}