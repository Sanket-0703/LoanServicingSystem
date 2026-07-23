using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreData.LoanOrigination
{
    [Table("[Disbursement]")]
    public class Disbursement
    {
        [Key]
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public decimal Principal { get; set; }
        public string BankAccount { get; set; } = string.Empty;
        public DateTime? TransactionDate { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }
}
