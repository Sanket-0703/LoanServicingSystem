using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CoreData.Identity
{
    [Table("[Users]")]
    public class Users
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? UpdatedBy { get; set; }

        public static async Task<Users?> GetUserByUsernameAsync(IDatabaseConnection databaseConnection, string username)
        {
            try
            {
                using var connection = databaseConnection.GetConnection();
                const string sql = @"
                    SELECT Id, RoleId, Username, PasswordHash, Email, IsActive, UpdatedBy 
                    FROM Users 
                    WHERE Username = @Username";

                return await connection.QueryFirstOrDefaultAsync<Users>(sql, new { Username = username });
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to fetch user record for username '{username}'. {ex.Message}", ex);
            }
        }

        public static async Task<Users?> GetUserByEmailAsync(IDatabaseConnection databaseConnection, string email)
        {
            try
            {
                using var connection = databaseConnection.GetConnection();
                const string sql = @"
            SELECT Id, RoleId, Username, PasswordHash, Email, IsActive, UpdatedBy 
            FROM [dbo].[Users] 
            WHERE Email = @Email"
                ;

                return await connection.QueryFirstOrDefaultAsync<Users>(sql, new { Email = email });
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to fetch user record for email '{email}'. {ex.Message}", ex);
            }
        }
    }
}
