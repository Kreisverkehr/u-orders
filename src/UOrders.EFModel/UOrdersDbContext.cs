using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UOrders.EFModel
{
    public class UOrdersDbContext : IdentityDbContext<UOrdersUser>
    {
        #region Public Constructors

        public UOrdersDbContext(DbContextOptions<UOrdersDbContext> options)
            : base(options)
        { }

        public UOrdersDbContext() : base()
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<MenuCategory> Categories => Set<MenuCategory>();

        public DbSet<MenuItem> MenuItems => Set<MenuItem>();

        public DbSet<MenuItemOption> MenuItemOptions => Set<MenuItemOption>();

        public DbSet<MenuItemOptionValue> MenuItemOptionValues => Set<MenuItemOptionValue>();

        public DbSet<Price> Prices => Set<Price>();

        public DbSet<LocalizedText> Texts => Set<LocalizedText>();

        public DbSet<Order> Orders => Set<Order>();

        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        public DbSet<OrderItemCheckedOption> OrderItemCheckedOptions => Set<OrderItemCheckedOption>();

        public DbSet<OrderReview> OrderReviews => Set<OrderReview>();

        #endregion Public Properties
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LocalizedText>()
                .HasOne(e => e.MenuCategoryTitle)
                .WithMany(e => e.Title)
                .OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<LocalizedText>()
                .HasOne(e => e.MenuCategoryDescription)
                .WithMany(e => e.Description)
                .OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<LocalizedText>()
                .HasOne(e => e.MenuItemTitle)
                .WithMany(e => e.Title)
                .OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<LocalizedText>()
                .HasOne(e => e.MenuItemDescription)
                .WithMany(e => e.Description)
                .OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<LocalizedText>()
                .HasOne(e => e.MenuItemOptionName)
                .WithMany(e => e.Name)
                .OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<LocalizedText>()
                .HasOne(e => e.MenuItemOptionDescription)
                .WithMany(e => e.Description)
                .OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<LocalizedText>()
                .HasOne(e => e.MenuItemOptionValueName)
                .WithMany(e => e.Name)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<MenuItemOptionValue>()
                .HasMany(e => e.PriceChangeToBase)
                .WithOne(e => e.MenuItemOptionValue)
                .OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<MenuItem>()
                .HasMany(e => e.Prices)
                .WithOne(e => e.MenuItem)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderedItems)
                .WithOne(e => e.Order)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Order>()
                .HasMany(e => e.Reviews)
                .WithOne(e => e.Order)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OrderItem>()
                .HasMany(e => e.CheckedOptions)
                .WithOne(e => e.OrderItem)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}