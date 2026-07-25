using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper;

namespace CoreData.LoanOrigination
{
    [Table("[LoanProduct]")]
    public class LoanProduct
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal? ProcessingFee { get; set; }
        public string? PenaltyRules { get; set; }
        public string? UpdatedBy { get; set; }

        public static async Task<List<LoanProduct>> GetAllAsync(IDatabaseConnection databaseConnection)
        {
            using var connection = databaseConnection.GetConnection();
            const string sql = "SELECT * FROM [dbo].[LoanProducts] ORDER BY Name";
            var result = await connection.QueryAsync<LoanProduct>(sql);
            return result.ToList();
        }

        public static async Task InsertAsync(IDatabaseConnection databaseConnection, LoanProduct product)
        {
            using var connection = databaseConnection.GetConnection();
            const string sql = @"
                INSERT INTO [dbo].[LoanProducts] 
                (Id, Name, InterestRate, TenureMonths, ProcessingFee, PenaltyRules, UpdatedBy)
                VALUES (@Id, @Name, @InterestRate, @TenureMonths, @ProcessingFee, @PenaltyRules, @UpdatedBy)";
            await connection.ExecuteAsync(sql, product);
        }

        public static async Task UpdateAsync(IDatabaseConnection databaseConnection, LoanProduct product)
        {
            using var connection = databaseConnection.GetConnection();

            const string sql = @"
                UPDATE [dbo].[LoanProducts]
                SET Name = @Name,
                    InterestRate = @InterestRate,
                    TenureMonths = @TenureMonths,
                    ProcessingFee = @ProcessingFee,
                    PenaltyRules = @PenaltyRules,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";
            await connection.ExecuteAsync(sql, product);
        }
    }
}
