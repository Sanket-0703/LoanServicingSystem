using System.Security.Claims;
using Dapper;
using Dapper.Contrib.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CoreData.Identity
{
    [Table("Users")]
    public class Users
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? UpdatedBy { get; set; }
        [Computed]
        public string RoleName { get; set; } = string.Empty;

        public static async Task<Users?> ValidateUserAsync(IDatabaseConnection databaseConnection, string username, string password)
        {
            using var connection = databaseConnection.GetConnection();

            // We join the Roles table here so we can attach the exact RoleName (e.g., "Admin") to your authentication cookie.
            const string sql = @"
        SELECT 
            u.Id, 
            u.RoleId, 
            u.Username, 
            u.Email, 
            u.IsActive, 
            u.PasswordHash,
            r.RoleName
        FROM [dbo].[Users] u
        LEFT JOIN [dbo].[Roles] r ON u.RoleId = r.Id
        WHERE u.Username = @Username AND u.PasswordHash = @Password";
            // Note: Checking PasswordHash as plaintext directly based on our previous setup

            return await connection.QueryFirstOrDefaultAsync<Users>(sql, new { Username = username, Password = password });
        }

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

        public static async Task<List<Users>> GetAllUsersWithRolesAsync(IDatabaseConnection databaseConnection)
        {
            using var connection = databaseConnection.GetConnection();
            const string sql = @"
        SELECT 
            u.Id, 
            u.Username, 
            u.Email, 
            u.IsActive, 
            r.RoleName 
        FROM [dbo].[Users] u
        LEFT JOIN [dbo].[Roles] r ON u.RoleId = r.Id
        ORDER BY u.Username";

            var result = await connection.QueryAsync<Users>(sql);
            return result.ToList();
        }

        public static async Task InsertUserAsync(IDatabaseConnection databaseConnection, Users user)
        {
            using var connection = databaseConnection.GetConnection();

            // Note: Since you are using Dapper.Contrib, you can usually just do connection.InsertAsync(user);
            // However, since [Computed] properties sometimes interfere with strict inserts, 
            // explicit SQL is safer for precise enterprise operations.
            const string sql = @"
                INSERT INTO [dbo].[Users] (Id, RoleId, Username, PasswordHash, Email, IsActive, UpdatedBy)
                VALUES (@Id, @RoleId, @Username, @PasswordHash, @Email, @IsActive, @UpdatedBy)";

            await connection.ExecuteAsync(sql, user);
        }

        public static async Task UpdateUserRoleAsync(IDatabaseConnection databaseConnection, Guid userId, Guid newRoleId, string updatedBy)
        {
            using var connection = databaseConnection.GetConnection();
            const string sql = @"
                UPDATE [dbo].[Users] 
                SET RoleId = @RoleId, UpdatedBy = @UpdatedBy 
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, new { Id = userId, RoleId = newRoleId, UpdatedBy = updatedBy });
        }

        public static async Task ToggleUserStatusAsync(IDatabaseConnection databaseConnection, Guid userId, bool isActive, string updatedBy)
        {
            using var connection = databaseConnection.GetConnection();
            const string sql = @"
                UPDATE [dbo].[Users] 
                SET IsActive = @IsActive, UpdatedBy = @UpdatedBy 
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, new { Id = userId, IsActive = isActive, UpdatedBy = updatedBy });
        }

        public static Guid GetCurrentUserId(ClaimsPrincipal user)
        {
            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.Parse(id!);
        }

        public static string GetCurrentUsername(ClaimsPrincipal user)
        {
            return user.Identity?.Name ?? "System";
        }

        public static string GetCurrentUserRole(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }


    }
}
