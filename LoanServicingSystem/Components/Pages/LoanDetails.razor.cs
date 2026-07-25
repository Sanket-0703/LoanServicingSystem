using CoreData;
using CoreData.LoanOrigination;
using CoreData.Servicing;
using Dapper;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class LoanDetails : ComponentBase
    {
        [Inject] private IDatabaseConnection? DatabaseConnection { get; set; }
        [Parameter] public Guid Id { get; set; }

        public bool IsLoading { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public string ActiveTab { get; set; } = "overview";

        public Loan? CurrentLoan { get; set; }
        public List<RepaymentSchedule> ScheduleItems { get; set; } = new();
        public List<LedgerTransactionItem> LedgerItems { get; set; } = new();

        // Computed Summary Metrics
        public decimal PrincipalBalance { get; set; }
        public decimal NextEmiAmount { get; set; }
        public DateTime? NextEmiDueDate { get; set; }
        public decimal TotalPaid { get; set; }
        public int PaidCount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                if (DatabaseConnection != null)
                {
                    using var connection = DatabaseConnection.GetConnection();

                    // 1. Fetch Loan with Customer and Product names
                    const string loanSql = @"
                        SELECT 
                            l.*,
                            c.Name AS CustomerName,
                            p.Name AS ProductName
                        FROM [dbo].[Loans] l
                        INNER JOIN [dbo].[Customers] c ON l.CustomerId = c.Id
                        INNER JOIN [dbo].[LoanProducts] p ON l.ProductId = p.Id
                        WHERE l.Id = @Id";

                    CurrentLoan = await connection.QueryFirstOrDefaultAsync<Loan>(loanSql, new { Id = Id });

                    if (CurrentLoan != null)
                    {
                        // 2. Fetch Schedule using RepaymentSchedule model
                        ScheduleItems = await RepaymentSchedule.GetByLoanIdAsync(DatabaseConnection, Id);

                        // 3. Fetch Disbursements and Payments for Ledger
                        var disbursements = await Disbursement.GetByLoanIdAsync(DatabaseConnection, Id);
                        var payments = await Payment.GetByLoanIdAsync(DatabaseConnection, Id);

                        // Compute Stats
                        PaidCount = ScheduleItems.Count(s => s.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase));
                        TotalPaid = payments.Sum(p => p.Amount);

                        var nextPending = ScheduleItems.FirstOrDefault(s => !s.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase));
                        if (nextPending != null)
                        {
                            NextEmiAmount = nextPending.Principal + nextPending.Interest;
                            NextEmiDueDate = nextPending.DueDate;
                            PrincipalBalance = nextPending.Outstanding + nextPending.Principal;
                        }
                        else
                        {
                            PrincipalBalance = 0;
                        }

                        if (PrincipalBalance == 0 && ScheduleItems.Any())
                        {
                            PrincipalBalance = ScheduleItems.Last().Outstanding;
                        }

                        // Build Ledger
                        BuildLedger(disbursements, payments);
                    }
                    else
                    {
                        ErrorMessage = "Loan account not found.";
                    }
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        private void BuildLedger(List<Disbursement> disbursements, List<Payment> payments)
        {
            var rawEvents = new List<LedgerEvent>();

            foreach (var d in disbursements)
            {
                rawEvents.Add(new LedgerEvent
                {
                    Timestamp = d.TransactionDate,
                    Type = "Principal Disbursement",
                    Debit = d.Principal,
                    Credit = null
                });
            }

            foreach (var p in payments)
            {
                rawEvents.Add(new LedgerEvent
                {
                    Timestamp = p.PaymentDate,
                    Type = string.IsNullOrWhiteSpace(p.PaymentType) ? "EMI Payment Received" : p.PaymentType,
                    Debit = null,
                    Credit = p.Amount
                });
            }

            var sorted = rawEvents.OrderBy(e => e.Timestamp).ToList();
            decimal runningBalance = 0;
            LedgerItems.Clear();

            foreach (var ev in sorted)
            {
                if (ev.Debit.HasValue) runningBalance += ev.Debit.Value;
                if (ev.Credit.HasValue) runningBalance -= ev.Credit.Value;

                LedgerItems.Add(new LedgerTransactionItem
                {
                    Timestamp = ev.Timestamp,
                    TransactionType = ev.Type,
                    Debit = ev.Debit,
                    Credit = ev.Credit,
                    RunningBalance = runningBalance
                });
            }
        }

        protected string GetStatusBadgeClass(string status) => status switch
        {
            "Disbursed" => "bg-emerald-100 text-emerald-800",
            "Approved" => "bg-blue-100 text-blue-800",
            "Draft" => "bg-slate-100 text-slate-800",
            "Closed" => "bg-purple-100 text-purple-800",
            _ => "bg-gray-100 text-gray-800"
        };

        private class LedgerEvent
        {
            public DateTime Timestamp { get; set; }
            public string Type { get; set; } = string.Empty;
            public decimal? Debit { get; set; }
            public decimal? Credit { get; set; }
        }

        public class LedgerTransactionItem
        {
            public DateTime Timestamp { get; set; }
            public string TransactionType { get; set; } = string.Empty;
            public decimal? Debit { get; set; }
            public decimal? Credit { get; set; }
            public decimal RunningBalance { get; set; }
        }
    }
}