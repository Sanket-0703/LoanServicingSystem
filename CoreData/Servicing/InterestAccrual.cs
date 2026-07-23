using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreData.Servicing
{
    [Table("[InterestAccrual]")]
    public class InterestAccrual
    {
        [Key]
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public decimal Outstanding { get; set; }
        public decimal Interest { get; set; }
        public DateTime AccrualDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
