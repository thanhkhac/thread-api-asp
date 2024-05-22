using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace thread_api_asp.Models;

[Table("users")]
[Index("Username", Name = "users_unique", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    [StringLength(36)]
    public string Id { get; set; } = null!;

    [Column("username")]
    [StringLength(100)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(100)]
    public string Password { get; set; } = null!;

    [Column("avatar")]
    [StringLength(900)]
    public string? Avatar { get; set; }

    [Column("create_at", TypeName = "timestamp")]
    public DateTime? CreateAt { get; set; }

    [Column("update_at", TypeName = "timestamp")]
    public DateTime? UpdateAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
