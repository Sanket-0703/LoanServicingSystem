using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreData.LoanOrigination
{
    [Table("[Loan]")]
    public class Loan
    {
        [Key]
        public Guid Id { get; set; }
        public string LoanNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public int Tenure { get; set; }
        public string? RepaymentFrequency { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
