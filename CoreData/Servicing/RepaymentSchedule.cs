using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreData.Servicing
{
    [Table("[RepaymentSchedule]")]
    public class RepaymentSchedule
    {
        [Key]
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public int EmiNo { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Outstanding { get; set; }
        public string? Status { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
