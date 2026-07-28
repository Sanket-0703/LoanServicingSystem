using CoreData;
using CoreData.Servicing;
using Dapper;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class RecordPayment : ComponentBase
    {
        [Inject] private IDatabaseConnection? DatabaseConnection { get; set; }
        [Inject] private NavigationManager? NavigationManager { get; set; }

        [Parameter] public Guid LoanId { get; set; }

        public bool IsLoading { get; set; } = true;
        public bool IsSaving { get; set; } = false;
        public string? ErrorMessage { get; set; }

        public string LoanNumber { get; set; } = string.Empty;
        public string BorrowerName { get; set; } = string.Empty;
        public decimal CurrentDueAmount { get; set; } = 0;

        public PaymentModel PaymentForm { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            try
            {
                if (DatabaseConnection != null)
                {
                    using var connection = DatabaseConnection.GetConnection();

                    // Fetch Loan & Customer info
                    const string sql = @"
                        SELECT 
                            l.LoanNumber,
                            c.Name AS BorrowerName
                        FROM [dbo].[Loans] l
                        INNER JOIN [dbo].[Customers] c ON l.CustomerId = c.Id
                        WHERE l.Id = @LoanId";

                    var loanInfo = await connection.QueryFirstOrDefaultAsync<LoanDto>(sql, new { LoanId = LoanId });
                    if (loanInfo != null)
                    {
                        LoanNumber = loanInfo.LoanNumber;
                        BorrowerName = loanInfo.BorrowerName;
                    }

                    // Fetch next pending EMI amount
                    const string dueSql = @"
                        SELECT TOP 1 (Principal + Interest) AS DueTotal 
                        FROM [dbo].[RepaymentSchedules] 
                        WHERE LoanId = @LoanId AND Status <> 'Paid' 
                        ORDER BY EmiNo ASC";

                    CurrentDueAmount = await connection.QueryFirstOrDefaultAsync<decimal>(dueSql, new { LoanId = LoanId });

                    // Set default form values
                    PaymentForm.Amount = CurrentDueAmount > 0 ? CurrentDueAmount : 0;
                    PaymentForm.PaymentType = "Standard EMI Installment";
                    PaymentForm.Mode = "Bank Wire Transfer (NEFT/RTGS)";
                    PaymentForm.PaymentDate = DateTime.Today;
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected async Task SubmitPaymentAsync()
        {
            ErrorMessage = null;

            if (PaymentForm.Amount <= 0)
            {
                ErrorMessage = "Please enter a valid payment amount greater than zero.";
                return;
            }

            IsSaving = true;
            try
            {
                if (DatabaseConnection == null) throw new Exception("Database connection missing.");

                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    LoanId = LoanId,
                    PaymentDate = PaymentForm.PaymentDate,
                    Amount = PaymentForm.Amount,
                    Mode = PaymentForm.Mode,
                    ReferenceNumber = PaymentForm.ReferenceNumber,
                    Remarks = PaymentForm.Remarks,
                    PaymentType = PaymentForm.PaymentType,
                    UpdatedBy = "SystemAdmin"
                };

                await Payment.RecordPaymentTransactionAsync(DatabaseConnection, payment);

                NavigationManager?.NavigateTo($"/loans/{LoanId}");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error posting payment: {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }

        public class PaymentModel
        {
            public decimal Amount { get; set; }
            public string PaymentType { get; set; } = "Standard EMI Installment";
            public string Mode { get; set; } = "Bank Wire Transfer (NEFT/RTGS)";
            public string? ReferenceNumber { get; set; }
            public string? Remarks { get; set; }
            public DateTime PaymentDate { get; set; }
        }

        private class LoanDto
        {
            public string LoanNumber { get; set; } = string.Empty;
            public string BorrowerName { get; set; } = string.Empty;
        }
    }
}