using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Data;

public class ApplicationDbContext : IdentityDbContext
{
    private IConfiguration Configuration { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IConfiguration configuration)
        : base(options)
    {
        Configuration = configuration;
    }

    public DbSet<ToDo> ToDos { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder builder)
    {

        string ADMIN_ID = Guid.NewGuid().ToString();
        string ADMIN_ROLE_ID = Guid.NewGuid().ToString();
        string USER_ROLE_ID = Guid.NewGuid().ToString();
        //seed admin role
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Name = "Admin",
            NormalizedName = "ADMIN",
            Id = ADMIN_ROLE_ID,
            ConcurrencyStamp = ADMIN_ROLE_ID
        });
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Name = "User",
            NormalizedName = "USER",
            Id = USER_ROLE_ID,
            ConcurrencyStamp = USER_ROLE_ID
        });
        string adminEmail = Configuration.GetValue<string>("Admin: Email") ?? "miluxas@gmail.com";
        //create user
        var appUser = new IdentityUser
        {
            Id = ADMIN_ID,
            Email = adminEmail,
            EmailConfirmed = true,
            UserName = adminEmail,
            NormalizedUserName = adminEmail.ToUpper(),
            SecurityStamp = "VWOJ3XKKB35CSPPR4NTIF4KGTGMHGJDF"
        };

        //set user password
        PasswordHasher<IdentityUser> ph = new PasswordHasher<IdentityUser>();
        appUser.PasswordHash = ph.HashPassword(appUser, Configuration.GetValue<string>("Admin: Password") ?? "!23Qaz");

        //seed user
        var res = builder.Entity<IdentityUser>().HasData(appUser);

        //set user role to admin
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = ADMIN_ROLE_ID,
            UserId = ADMIN_ID
        });



        base.OnModelCreating(builder);

    }
}

