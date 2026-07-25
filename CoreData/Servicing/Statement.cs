using Dapper;
using Dapper.Contrib.Extensions;

namespace CoreData.Servicing
{
    [Table("Statements")]
    public class Statement
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public string StatementType { get; set; } = "Full Account Ledger Statement";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? UpdatedBy { get; set; }

        public static async Task<StatementDetailsDto?> GetStatementDataAsync(IDatabaseConnection databaseConnection, Guid loanId, DateTime startDate, DateTime endDate)
        {
            using var connection = databaseConnection.GetConnection();

            // 1. Fetch Loan, Customer, and Product info
            const string loanSql = @"
                SELECT 
                    l.Id AS LoanId,
                    l.LoanNumber,
                    l.Principal,
                    l.InterestRate,
                    c.Name AS BorrowerName,
                    c.Id AS CustomerId,
                    c.Email AS BorrowerEmail,
                    p.Name AS ProductName
                FROM [dbo].[Loans] l
                INNER JOIN [dbo].[Customers] c ON l.CustomerId = c.Id
                INNER JOIN [dbo].[LoanProducts] p ON l.ProductId = p.Id
                WHERE l.Id = @LoanId";

            var meta = await connection.QueryFirstOrDefaultAsync<StatementMetadataDto>(loanSql, new { LoanId = loanId });
            if (meta == null) return null;

            // 2. Fetch Disbursements within date range (or all if range is broad)
            const string disbSql = "SELECT TransactionDate AS Timestamp, 'Principal Disbursement' AS TransactionType, Principal AS Debit, CAST(NULL AS DECIMAL(18,2)) AS Credit FROM [dbo].[Disbursements] WHERE LoanId = @LoanId AND TransactionDate >= @StartDate AND TransactionDate <= @EndDate";
            var disbursements = await connection.QueryAsync<StatementRowDto>(disbSql, new { LoanId = loanId, StartDate = startDate, EndDate = endDate });

            // 3. Fetch Payments within date range
            const string paySql = "SELECT PaymentDate AS Timestamp, ISNULL(PaymentType, 'EMI Payment Received') AS TransactionType, CAST(NULL AS DECIMAL(18,2)) AS Debit, Amount AS Credit FROM [dbo].[Payments] WHERE LoanId = @LoanId AND PaymentDate >= @StartDate AND PaymentDate <= @EndDate";
            var payments = await connection.QueryAsync<StatementRowDto>(paySql, new { LoanId = loanId, StartDate = startDate, EndDate = endDate });

            // Combine and sort chronologically
            var rawRows = disbursements.Concat(payments).OrderBy(r => r.Timestamp).ToList();

            decimal runningBalance = 0;
            var processedRows = new List<StatementRowDto>();
            foreach (var row in rawRows)
            {
                if (row.Debit.HasValue) runningBalance += row.Debit.Value;
                if (row.Credit.HasValue) runningBalance -= row.Credit.Value;

                processedRows.Add(new StatementRowDto
                {
                    Timestamp = row.Timestamp,
                    TransactionType = row.TransactionType,
                    Debit = row.Debit,
                    Credit = row.Credit,
                    RunningBalance = runningBalance
                });
            }

            return new StatementDetailsDto
            {
                Metadata = meta,
                Rows = processedRows
            };
        }
    }

    public class StatementMetadataDto
    {
        public Guid LoanId { get; set; }
        public string LoanNumber { get; set; } = string.Empty;
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public string BorrowerName { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string? BorrowerEmail { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }

    public class StatementRowDto
    {
        public DateTime Timestamp { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal RunningBalance { get; set; }
    }

    public class StatementDetailsDto
    {
        public StatementMetadataDto Metadata { get; set; } = new();
        public List<StatementRowDto> Rows { get; set; } = new();
    }
}