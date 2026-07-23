using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreData.LoanOrigination
{
    [Table("[LoanProduct]")]
    public class LoanProduct
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal? ProcessingFee { get; set; }
        public string? PenaltyRules { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
