using ECommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Identity.Data
{
    public class StoreIdentityDbContext(DbContextOptions<StoreIdentityDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Address> Addresses => Set<Address>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Ignore<IdentityUserClaim<string>>();
            builder.Ignore<IdentityUserLogin<string>>();
            builder.Ignore<IdentityUserToken<string>>();
            builder.Ignore<IdentityRoleClaim<string>>();

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

            builder.Entity<Address>(address =>
            {
                address.ToTable("Addresses");

                address.HasKey(a => a.Id);

                address.Property(a => a.Label)
                    .IsRequired()
                    .HasMaxLength(50);

                address.Property(a => a.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                address.Property(a => a.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                address.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(100);

                address.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(100);

                address.Property(a => a.Street)
                    .IsRequired()
                    .HasMaxLength(200);

                address.Property(a => a.Building)
                    .HasMaxLength(100);

                address.Property(a => a.Notes)
                    .HasMaxLength(300);

                address.HasIndex(a => a.UserId);

                address.HasOne(a => a.User)
                    .WithMany(u => u.Addresses)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
