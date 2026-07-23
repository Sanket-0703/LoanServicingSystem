using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreData.Servicing
{
    [Table("[Payment]")]
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string? Mode { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Remarks { get; set; }
        public string? PaymentType { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
