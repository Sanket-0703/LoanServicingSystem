using CoreData;
using CoreData.Dashboard.Models;
using CoreData.Export;
using CoreData.LoanOrigination;
using CoreData.Servicing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LoanServicingSystem.Components.Pages
{
    public partial class LoanDetails : ComponentBase
    {
        // =========================================
        // Dependency Injection
        // =========================================

        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        // =========================================
        // Route Parameters
        // =========================================

        [Parameter]
        public Guid Id { get; set; }

        // =========================================
        // Page Data
        // =========================================

        public bool IsLoading { get; set; } = false;

        public string? ErrorMessage { get; set; }

        public string ActiveTab { get; set; } = "overview";

        public Loan? CurrentLoan { get; set; }

        public List<RepaymentSchedule> ScheduleItems { get; set; } = new();

        public List<LedgerTransactionItem> LedgerItems { get; set; } = new();
        public Payment PaymentSummary { get; set; } = new();

        public List<Payment> Payments { get; set; } = new();
        public List<AuditTrailModel> AuditRecords { get; set; } = new();

        // =========================================
        // Summary Metrics
        // =========================================

        public decimal PrincipalBalance { get; set; }

        public decimal NextEmiAmount { get; set; }

        public DateTime? NextEmiDueDate { get; set; }

        public decimal TotalPaid { get; set; }

        public int PaidCount { get; set; }

        // =========================================
        // Modal State
        // =========================================

        public bool ShowDisbursementModal { get; set; }

        public bool ShowConfirmationModal { get; set; }

        public string ConfirmationTitle { get; set; } = string.Empty;

        public string ConfirmationMessage { get; set; } = string.Empty;

        public Func<Task>? ConfirmationAction { get; set; }

        public Disbursement NewDisbursement { get; set; } = new();

        // =========================================
        // Lifecycle Methods
        // =========================================

        /// <summary>
        /// Loads the loan details when the page is initialized.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            ErrorMessage = null;
            await LoadDataAsync();
        }

        // =========================================
        // Data Loading
        // =========================================

        /// <summary>
        /// Loads the loan, repayment schedule, disbursements,
        /// payments, and computes dashboard metrics.
        /// </summary>
        // Use OnParametersSetAsync instead of OnInitializedAsync for routed parameters
        //protected override async Task OnParametersSetAsync()
        //{
        //    // Reset state when the ID changes
        //    //ErrorMessage = null;
        //    await LoadDataAsync();
        //}

        private async Task LoadDataAsync()
        {
            IsLoading = true;
            ErrorMessage = null;

            // Force the UI to render the "Loading..." message immediately


            try
            {
                if (DatabaseConnection != null)
                {
                    // Removed Task.Run! Safe to execute directly now that Prerendering is off.
                    CurrentLoan = await Loan.GetLoanWorkspaceAsync(DatabaseConnection, Id);
                    Console.WriteLine("Loan OK");

                    if (CurrentLoan != null)
                    {
                        // Fetch data safely
                        ScheduleItems = await RepaymentSchedule.GetByLoanIdAsync(DatabaseConnection, Id) ?? new();
                        Console.WriteLine("Schedule OK");

                        var disbursements = await Disbursement.GetByLoanIdAsync(DatabaseConnection, Id) ?? new();

                        PaymentSummary = await Payment.GetPaymentScreenDataAsync(DatabaseConnection, Id) ?? new();
                        Console.WriteLine("PaymentSummary OK");
                        Payments = await Payment.GetByLoanIdAsync(DatabaseConnection, Id) ?? new();
                        Console.WriteLine("Payments OK");
                        AuditRecords = await AuditTrailModel.GetByLoanIdAsync(DatabaseConnection, Id) ?? new();
                        Console.WriteLine("Audit OK");

                        // Safe LINQ calculations
                        PaidCount = ScheduleItems.Count(s => string.Equals(s.Status, "Paid", StringComparison.OrdinalIgnoreCase));
                        TotalPaid = PaymentSummary.TotalCollected;

                        var nextPending = ScheduleItems.FirstOrDefault(s => !string.Equals(s.Status, "Paid", StringComparison.OrdinalIgnoreCase));

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

                        BuildLedger(disbursements, Payments);
                    }
                    else
                    {
                        ErrorMessage = $"Loan record not found. Requested ID: {Id} | DB State: {DatabaseConnection?.GetConnection().Database}";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"System Error: {ex.Message}";
                Console.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
                // Update the UI to remove the loading screen and show the data/error
                //StateHasChanged();
            }
        }

        // Make BuildLedger null-safe just in case
        private void BuildLedger(List<Disbursement>? disbursements, List<Payment>? payments)
        {
            var rawEvents = new List<LedgerEvent>();

            if (disbursements != null)
            {
                foreach (var d in disbursements)
                {
                    rawEvents.Add(new LedgerEvent
                    {
                        Timestamp = d.TransactionDate,
                        Type = "Principal Disbursement",
                        Debit = d.Principal
                    });
                }
            }

            if (payments != null)
            {
                foreach (var p in payments)
                {
                    rawEvents.Add(new LedgerEvent
                    {
                        Timestamp = p.PaymentDate,
                        Type = string.IsNullOrWhiteSpace(p.PaymentType) ? "EMI Payment Received" : p.PaymentType,
                        Credit = p.Amount
                    });
                }
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

        private async Task ExportStatement()
        {
            var workbook = LoanStatementExporter.GenerateWorkbook(
                CurrentLoan!,
                Payments,
                ScheduleItems);

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            var bytes = stream.ToArray();

            await JS.InvokeVoidAsync(
                "downloadFile",
                $"LoanStatement_{CurrentLoan!.LoanNumber}.xlsx",
                Convert.ToBase64String(bytes));
        }

        // =========================================
        // Data Processing
        // =========================================

        /// <summary>
        /// Builds the transaction ledger with running balance.
        /// </summary>


        // =========================================
        // Loan Actions
        // =========================================

        /// <summary>
        /// Opens the approval confirmation dialog.
        /// </summary>
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

        /// <summary>
        /// Opens the rejection confirmation dialog.
        /// </summary>
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

        /// <summary>
        /// Opens the loan disbursement dialog.
        /// </summary>
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

        /// <summary>
        /// Saves the disbursement and updates loan status.
        /// </summary>
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

        // =========================================
        // Modal Actions
        // =========================================

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

        protected void CloseDisbursementModal()
        {
            ShowDisbursementModal = false;
        }

        // =========================================
        // UI Helper Methods
        // =========================================

        protected string GetStatusBadgeClass(string status) =>
            status switch
            {
                "Disbursed" => "bg-emerald-100 text-emerald-800",
                "Approved" => "bg-blue-100 text-blue-800",
                "Draft" => "bg-slate-100 text-slate-800",
                "Closed" => "bg-purple-100 text-purple-800",
                _ => "bg-gray-100 text-gray-800"
            };

        // =========================================
        // Helper Classes
        // =========================================

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