using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreData.Servicing
{
    [Table("[Penalty]")]
    public class Penalty
    {
        [Key]
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? AppliedDate { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
