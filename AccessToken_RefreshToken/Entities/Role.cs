using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace thread_api_asp.Entities;

[Table("roles")]
[Index("Prefix", Name = "roles_unique", IsUnique = true)]
public partial class Role
{
    [Key]
    [Column("id")]
    [StringLength(100)]
    public string Id { get; set; } = null!;

    [Column("prefix")]
    [StringLength(100)]
    public string? Prefix { get; set; }

    [Column("is_default")]
    public bool? IsDefault { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
