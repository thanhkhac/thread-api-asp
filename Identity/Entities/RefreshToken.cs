using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Entities
{
    [Table("refreshtoken")]
    public class RefreshToken
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = null!;

        [StringLength(36)]
        public string? UserId { get; set; }

        [StringLength(5000)]
        public string Token { get; set; } = null!;

        [StringLength(100)]
        public string JwtId { get; set; } = null!;

        public bool? IsRevoked { get; set; }

        public bool? IsUsed { get; set; }

        public DateTime? IssuedAt { get; set; }

        public DateTime? ExpiredAt { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("RefreshTokens")]
        public virtual User? User { get; set; }
    }
}