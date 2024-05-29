using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Identity.Entities
{
    public class User : IdentityUser
    {
        [StringLength(100)]
        public string? FullName { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}