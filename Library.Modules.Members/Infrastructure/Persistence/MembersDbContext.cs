using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Library.Modules.Members.Domain;

namespace Library.Modules.Members.Infrastructure.Persistence;

public class MembersDbContext(DbContextOptions<MembersDbContext> options)
    : IdentityDbContext<Member, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Member> Members => Set<Member>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Scope all tables in this DbContext to the dedicated PostgreSQL schema
        builder.HasDefaultSchema("members");

        // Apply custom entity configurations
        builder.ApplyConfigurationsFromAssembly(typeof(MembersDbContext).Assembly);

        // Rename default Identity framework tables to clean, professional names in the 'members' schema
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}
