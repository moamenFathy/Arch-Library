using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Library.Modules.Members.Application.Members;
using Library.Modules.Members.Domain;
using Library.Modules.Members.Infrastructure.Persistence;

namespace Library.Modules.Members;

public static class MembersModule
{
    public static IServiceCollection AddMembersModule(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<MembersDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "members")));

        services.AddIdentityCore<Member>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<MembersDbContext>();

        // Validators
        services.AddValidatorsFromAssembly(typeof(MembersModule).Assembly);

        // Application services
        services.AddScoped<IMemberService, MemberService>();

        return services;
    }
}
