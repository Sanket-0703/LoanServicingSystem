using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreData.CustomerManagement
{
    [Table("[Customer]")]
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string PAN { get; set; } = string.Empty;
        public string? Aadhaar { get; set; }
        public string? BusinessName { get; set; }
        public string? EmploymentType { get; set; }
        public decimal? Income { get; set; }
        public int? CreditScore { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
