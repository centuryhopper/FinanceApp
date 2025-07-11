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

    public virtual DbSet<Bankinfo> Bankinfos { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Plaiditem> Plaiditems { get; set; }

    public virtual DbSet<Streamlinedtransaction> Streamlinedtransactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bankinfo>(entity =>
        {
            entity.HasKey(e => e.Bankinfoid).HasName("bankinfo_pkey");

            entity.ToTable("bankinfo");

            entity.Property(e => e.Bankinfoid).HasColumnName("bankinfoid");
            entity.Property(e => e.Bankname).HasColumnName("bankname");
            entity.Property(e => e.Totalbankbalance)
                .HasPrecision(10, 2)
                .HasColumnName("totalbankbalance");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Bankinfos)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bankinfo_userid_fkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("category_pkey");

            entity.ToTable("category");

            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.CategoryBudget).HasColumnName("category_budget");
            entity.Property(e => e.Name).HasColumnName("name");

            entity.HasMany(d => d.Streamlinedtransactions).WithMany(p => p.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "CategorytransactionJunc",
                    r => r.HasOne<Streamlinedtransaction>().WithMany()
                        .HasForeignKey("Streamlinedtransactionsid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("categorytransaction_junc_streamlinedtransactionsid_fkey"),
                    l => l.HasOne<Category>().WithMany()
                        .HasForeignKey("Categoryid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("categorytransaction_junc_categoryid_fkey"),
                    j =>
                    {
                        j.HasKey("Categoryid", "Streamlinedtransactionsid").HasName("categorytransaction_junc_pkey");
                        j.ToTable("categorytransaction_junc");
                        j.IndexerProperty<int>("Categoryid").HasColumnName("categoryid");
                        j.IndexerProperty<int>("Streamlinedtransactionsid").HasColumnName("streamlinedtransactionsid");
                    });
        });

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

        modelBuilder.Entity<Streamlinedtransaction>(entity =>
        {
            entity.HasKey(e => e.Streamlinedtransactionsid).HasName("streamlinedtransactions_pkey");

            entity.ToTable("streamlinedtransactions");

            entity.Property(e => e.Streamlinedtransactionsid).HasColumnName("streamlinedtransactionsid");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Bankinfoid).HasColumnName("bankinfoid");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Environmenttype).HasColumnName("environmenttype");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.Transactionid).HasColumnName("transactionid");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Bankinfo).WithMany(p => p.Streamlinedtransactions)
                .HasForeignKey(d => d.Bankinfoid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_streamlinedtransactions_bankinfo");

            entity.HasOne(d => d.User).WithMany(p => p.Streamlinedtransactions)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("streamlinedtransactions_userid_fkey");
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
