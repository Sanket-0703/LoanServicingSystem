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

        public bool ShowDisbursementModal { get; set; }

        public bool ShowConfirmationModal { get; set; }

        public string ConfirmationTitle { get; set; } = string.Empty;

        public string ConfirmationMessage { get; set; } = string.Empty;

        public Func<Task>? ConfirmationAction { get; set; }
        public Disbursement NewDisbursement { get; set; } = new();
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

        protected Task ApproveLoan()
        {
            ConfirmationTitle = "Approve Loan";

            ConfirmationMessage =
                $"Are you sure you want to approve loan '{CurrentLoan?.LoanNumber}'?";

            ConfirmationAction = ConfirmApproveLoan;

            ShowConfirmationModal = true;

            return Task.CompletedTask;
        }

        private async Task ConfirmApproveLoan()
        {
            try
            {
                if (CurrentLoan == null || DatabaseConnection == null)
                    return;

                await Loan.ApproveAsync(
                    DatabaseConnection,
                    CurrentLoan.Id,
                    "SystemAdmin");

                ShowConfirmationModal = false;

                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                ErrorMessage = ex.ToString();

                ShowConfirmationModal = false;

                StateHasChanged();
            }
        }
        protected Task RejectLoan()
        {
            ConfirmationTitle = "Reject Loan";

            ConfirmationMessage =
                $"Are you sure you want to reject loan '{CurrentLoan?.LoanNumber}'?";

            ConfirmationAction = ConfirmRejectLoan;

            ShowConfirmationModal = true;

            return Task.CompletedTask;
        }

        private async Task ConfirmRejectLoan()
        {
            if (CurrentLoan == null || DatabaseConnection == null)
                return;

            await Loan.RejectAsync(
                DatabaseConnection,
                CurrentLoan.Id,
                "SystemAdmin");

            ShowConfirmationModal = false;

            await LoadDataAsync();
        }

        protected Task DisburseLoan()
        {
            if (CurrentLoan == null)
                return Task.CompletedTask;

            NewDisbursement = new Disbursement
            {
                LoanId = CurrentLoan.Id,
                Principal = CurrentLoan.Principal,
                TransactionDate = DateTime.Now
            };

            ShowDisbursementModal = true;

            return Task.CompletedTask;
        }

        protected void CloseDisbursementModal()
        {
            ShowDisbursementModal = false;
        }

        protected async Task ExecuteConfirmation()
        {
            if (ConfirmationAction != null)
            {
                await ConfirmationAction();
            }
        }

        protected void CloseConfirmationModal()
        {
            ShowConfirmationModal = false;
        }

        protected async Task ConfirmDisbursement()
        {
            if (DatabaseConnection == null || CurrentLoan == null)
                return;

            try
            {
                NewDisbursement.Id = Guid.NewGuid();
                NewDisbursement.UpdatedBy = "SystemAdmin";

                if (string.IsNullOrWhiteSpace(NewDisbursement.BankAccount))
                {
                    ErrorMessage = "Bank Account is required.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewDisbursement.ReferenceNumber))
                {
                    ErrorMessage = "Reference Number is required.";
                    return;
                }

                await Disbursement.InsertAsync(
                    DatabaseConnection,
                    NewDisbursement);

                await Loan.DisburseAsync(
                    DatabaseConnection,
                    CurrentLoan.Id,
                    "SystemAdmin");

                ShowDisbursementModal = false;

                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
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