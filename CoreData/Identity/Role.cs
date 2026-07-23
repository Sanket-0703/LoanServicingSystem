using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreData.Identity
{
    [Table("[Role]")]
    public class Role
    {
        [Key]
        public Guid Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }
}
