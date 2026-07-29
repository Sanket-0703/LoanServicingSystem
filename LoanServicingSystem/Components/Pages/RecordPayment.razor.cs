using CoreData;
using CoreData.Servicing;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Pages
{
    public partial class RecordPayment : ComponentBase
    {
        // =========================================
        // Dependency Injection
        // =========================================

        [Inject]
        private IDatabaseConnection? DatabaseConnection { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // =========================================
        // Route Parameters
        // =========================================

        [Parameter]
        public Guid LoanId { get; set; }

        // =========================================
        // Page State
        // =========================================

        public bool IsLoading { get; set; } = true;

        public bool IsSaving { get; set; }

        public string? ErrorMessage { get; set; }

        // =========================================
        // Loan Information
        // =========================================

        public string LoanNumber { get; set; } = string.Empty;

        public string BorrowerName { get; set; } = string.Empty;

        public decimal CurrentDueAmount { get; set; }

        // =========================================
        // Payment Form
        // =========================================

        public PaymentModel PaymentForm { get; set; } = new();

        // =========================================
        // Lifecycle Methods
        // =========================================

        /// <summary>
        /// Loads loan information and initializes
        /// the payment form.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            try
            {
                if (DatabaseConnection != null)
                {
                    var paymentData = await Payment.GetPaymentScreenDataAsync(
                        DatabaseConnection,
                        LoanId);

                    if (paymentData != null)
                    {
                        LoanNumber = paymentData.LoanNumber;
                        BorrowerName = paymentData.BorrowerName;
                        CurrentDueAmount = paymentData.CurrentDueAmount;
                    }

                    // Initialize default payment values
                    PaymentForm.Amount =
                        CurrentDueAmount > 0
                            ? CurrentDueAmount
                            : 0;

                    PaymentForm.PaymentType =
                        "Standard EMI Installment";

                    PaymentForm.Mode =
                        "Bank Wire Transfer (NEFT/RTGS)";

                    PaymentForm.PaymentDate =
                        DateTime.Today;
                }
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        // =========================================
        // Payment Processing
        // =========================================

        /// <summary>
        /// Records the payment transaction and
        /// redirects back to the loan details page.
        /// </summary>
        protected async Task SubmitPaymentAsync()
        {
            ErrorMessage = null;

            // Validate payment amount
            if (PaymentForm.Amount <= 0)
            {
                ErrorMessage =
                    "Please enter a valid payment amount greater than zero.";

                return;
            }

            IsSaving = true;

            try
            {
                if (DatabaseConnection == null)
                    throw new Exception("Database connection missing.");

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

                await Payment.RecordPaymentTransactionAsync(
                    DatabaseConnection,
                    payment);

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

        // =========================================
        // View Models
        // =========================================

        /// <summary>
        /// Represents the payment form displayed
        /// on the page.
        /// </summary>
        public class PaymentModel
        {
            public decimal Amount { get; set; }

            public string PaymentType { get; set; }
                = "Standard EMI Installment";

            public string Mode { get; set; }
                = "Bank Wire Transfer (NEFT/RTGS)";

            public string? ReferenceNumber { get; set; }

            public string? Remarks { get; set; }

            public DateTime PaymentDate { get; set; }
        }
    }
}