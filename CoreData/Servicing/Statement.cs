using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreData.Servicing
{
    [Table("[Statement]")]
    public class Statement
    {
        [Key]
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? ClosingBalance { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public string? FilePath { get; set; }
        public string? UpdatedBy { get; set; }

    }
}
