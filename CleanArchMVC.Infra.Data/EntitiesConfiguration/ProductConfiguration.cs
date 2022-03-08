﻿using CleanArchMVC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchMVC.Infra.Data.EntitiesConfiguration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).HasMaxLength(150).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(250).IsRequired();
            builder.Property(p => p.Price).HasPrecision(10, 2);

            builder.HasOne(p => p.Category).WithMany(p => p.Products)
                .HasForeignKey(p => p.CategoryId);


        }
    }
}
