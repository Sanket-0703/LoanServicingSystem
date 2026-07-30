using Dapper;
using Dapper.Contrib.Extensions;

namespace CoreData.CustomerManagement
{
    [Table("Customers")]
    public class Customer
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string PAN { get; set; } = string.Empty;
        public string? Aadhaar { get; set; }
        public string? BusinessName { get; set; }
        public string? EmploymentType { get; set; }
        public decimal? Income { get; set; }
        public int? CreditScore { get; set; }
        public Guid? CreatedBy { get; set; }



        [Computed]
        public int ActiveLoansCount { get; set; }

        [Computed]
        public string KycStatus
        {
            get
            {

                if (!string.IsNullOrWhiteSpace(PAN) && !string.IsNullOrWhiteSpace(Aadhaar))
                    return "Verified";

                return "Pending";
            }
        }



        public static async Task<List<Customer>> GetAllCustomerUderLoanOfficer(IDatabaseConnection databaseConnection, Guid LoanofficerId)
        {
            using var connection = databaseConnection.GetConnection();


            const string sql = @"
                SELECT * FROM Customers c
where c.CreatedBy=@LoanofficerId
or '8CD71F9D-E28A-F111-A099-FD86528A1448'=@LoanofficerId";

            var result = await connection.QueryAsync<Customer>(sql, new { LoanofficerId });
            return result.ToList();
        }

        public static async Task<Customer?> GetByIdAsync(IDatabaseConnection databaseConnection, Guid id)
        {
            using var connection = databaseConnection.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<Customer>("SELECT * FROM [dbo].[Customers] WHERE Id = @Id", new { Id = id });
        }

        public static async Task InsertAsync(IDatabaseConnection databaseConnection, Customer customer)
        {
            using var connection = databaseConnection.GetConnection();
            const string sql = @"
                INSERT INTO [dbo].[Customers] 
                (Id, Name, Email, Phone, Address, PAN, Aadhaar, BusinessName, EmploymentType, Income, CreditScore, CreatedBy)
                VALUES (@Id, @Name, @Email, @Phone, @Address, @PAN, @Aadhaar, @BusinessName, @EmploymentType, @Income, @CreditScore, @CreatedBy)";
            await connection.ExecuteAsync(sql, customer);
        }

        public static async Task UpdateAsync(IDatabaseConnection databaseConnection, Customer customer)
        {
            using var connection = databaseConnection.GetConnection();
            const string sql = @"
                UPDATE [dbo].[Customers]
                SET Name = @Name, Email = @Email, Phone = @Phone, Address = @Address, 
                    PAN = @PAN, Aadhaar = @Aadhaar, BusinessName = @BusinessName, 
                    EmploymentType = @EmploymentType, Income = @Income, 
                    CreditScore = @CreditScore, CreatedBy = @CreatedBy
                WHERE Id = @Id";
            await connection.ExecuteAsync(sql, customer);
        }
    }
}