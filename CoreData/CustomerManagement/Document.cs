using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreData.CustomerManagement
{
    [Table("[Document]")]
    public class Document
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? LoanId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime? UploadedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
