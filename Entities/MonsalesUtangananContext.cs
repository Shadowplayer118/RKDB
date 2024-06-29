using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RKDB.Entities;

public partial class MonsalesUtangananContext : DbContext
{
    public MonsalesUtangananContext()
    {
    }

    public MonsalesUtangananContext(DbContextOptions<MonsalesUtangananContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Clientinfo> Clientinfos { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Usertype> Usertypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\OTEN;Database=monsalesUtanganan;TrustServerCertificate=true;Trusted_Connection=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Clientinfo>(entity =>
        {
            entity.ToTable("clientinfo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Birthday)
                .HasColumnType("date")
                .HasColumnName("birthday");
            entity.Property(e => e.CivilStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("civil_status");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastname");
            entity.Property(e => e.Middlename)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("middlename");
            entity.Property(e => e.Nameoffather)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nameoffather");
            entity.Property(e => e.Nameofmother)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nameofmother");
            entity.Property(e => e.Occupation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("occupation");
            entity.Property(e => e.Religion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("religion");
            entity.Property(e => e.Usertype).HasColumnName("usertype");
            entity.Property(e => e.Zipcode).HasColumnName("zipcode");
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.ToTable("loan");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasColumnName("dateCreated");
            entity.Property(e => e.Deduction)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("deduction");
            entity.Property(e => e.Duedate)
                .HasColumnType("date")
                .HasColumnName("duedate");
            entity.Property(e => e.Interest).HasColumnName("interest");
            entity.Property(e => e.PayableAmount)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("payableAmount");
            entity.Property(e => e.Payment)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payment");
            entity.Property(e => e.PaymentAmount)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("paymentAmount");
            entity.Property(e => e.Penalty).HasColumnName("penalty");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Term).HasColumnName("term");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("total");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("schedule");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.LoanId).HasColumnName("loan_id");
            entity.Property(e => e.PaymentAmount)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("paymentAmount");
            entity.Property(e => e.ScheduleDate)
                .HasColumnType("datetime")
                .HasColumnName("schedule_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("transaction");

            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.PaymentAmount)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("paymentAmount");
            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.TransactionDate)
                .HasColumnType("datetime")
                .HasColumnName("transaction_date");
        });

        modelBuilder.Entity<Usertype>(entity =>
        {
            entity.ToTable("usertype");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
