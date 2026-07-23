using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreData.Identity
{
    [Table("[User]")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
