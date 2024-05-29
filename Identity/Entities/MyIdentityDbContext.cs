using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Entities
{
    public class MyIdentityDbContext : IdentityDbContext<User>
    {
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public MyIdentityDbContext(DbContextOptions<MyIdentityDbContext> options)
            : base(options)
        {
        }
    }
}