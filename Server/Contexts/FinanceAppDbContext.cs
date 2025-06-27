using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Server.Entities;

namespace Server.Contexts;

public partial class FinanceAppDbContext : DbContext
{
    public FinanceAppDbContext()
    {
    }

    public FinanceAppDbContext(DbContextOptions<FinanceAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Plaiditem> Plaiditems { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Plaiditem>(entity =>
        {
            entity.HasKey(e => e.Plaiditemid).HasName("plaiditems_pkey");

            entity.ToTable("plaiditems");

            entity.Property(e => e.Plaiditemid).HasColumnName("plaiditemid");
            entity.Property(e => e.AccessTokenIv).HasColumnName("access_token_iv");
            entity.Property(e => e.Accesstoken).HasColumnName("accesstoken");
            entity.Property(e => e.Datelinked)
                .HasDefaultValueSql("now()")
                .HasColumnName("datelinked");
            entity.Property(e => e.Institutionname).HasColumnName("institutionname");
            entity.Property(e => e.TransactionsCursor).HasColumnName("transactions_cursor");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Plaiditems)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("plaiditems_userid_fkey");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Transactionsid).HasName("transactions_pkey");

            entity.ToTable("transactions");

            entity.Property(e => e.Transactionsid).HasColumnName("transactionsid");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Balance)
                .HasPrecision(10, 2)
                .HasColumnName("balance");
            entity.Property(e => e.Checkorslip).HasColumnName("checkorslip");
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .HasColumnName("description");
            entity.Property(e => e.Details)
                .HasMaxLength(15)
                .HasColumnName("details");
            entity.Property(e => e.Postingdate).HasColumnName("postingdate");
            entity.Property(e => e.Type)
                .HasMaxLength(64)
                .HasColumnName("type");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("transactions_userid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.UmsUserid, "users_ums_userid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Datecreated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("datecreated");
            entity.Property(e => e.Datelastlogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("datelastlogin");
            entity.Property(e => e.Datelastlogout)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("datelastlogout");
            entity.Property(e => e.Dateretired)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("dateretired");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(256)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(256)
                .HasColumnName("lastname");
            entity.Property(e => e.UmsUserid)
                .HasMaxLength(450)
                .HasColumnName("ums_userid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
