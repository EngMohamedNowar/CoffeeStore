using CoffeeStore.Domain.Entities.Addresses;
using CoffeeStore.Domain.Entities.Carts;
using CoffeeStore.Domain.Entities.Categories;
using CoffeeStore.Domain.Entities.Customers;
using CoffeeStore.Domain.Entities.Orders;
using CoffeeStore.Domain.Entities.Payments;
using CoffeeStore.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Persistence.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // لازم أول سطر عشان جداول الـ Identity تتظبط

            // بيطبق كل الـ IEntityTypeConfiguration<T> الموجودة في الـ Assembly ده تلقائيًا
            builder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly);
        }
    }
}
