using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace thread_api_asp.Entities;

[Table("refresh_token")]
public partial class RefreshToken
{
    [Key]
    [Column("id")]
    [StringLength(36)]
    public string Id { get; set; } = null!;

    [Column("user_id")]
    [StringLength(36)]
    public string? UserId { get; set; }

    [Column("token")]
    [StringLength(5000)]
    public string Token { get; set; } = null!;

    [Column("jwt_id")]
    [StringLength(100)]
    public string JwtId { get; set; } = null!;

    [Column("is_revoked")]
    public bool? IsRevoked { get; set; }

    [Column("is_used")]
    public bool? IsUsed { get; set; }

    [Column("issued_at", TypeName = "datetime")]
    public DateTime? IssuedAt { get; set; }

    [Column("expired_at", TypeName = "datetime")]
    public DateTime? ExpiredAt { get; set; }
}
