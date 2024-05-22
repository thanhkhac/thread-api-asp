namespace thread_api_asp.Models;

public partial class Role
{
    public string Id { get; set; } = null!;

    public string? Prefix { get; set; }

    public bool? IsDefault { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
