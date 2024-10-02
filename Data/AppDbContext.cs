using System;
using System.Collections.Generic;
using HandsForPeaceMakingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HandsForPeaceMakingAPI.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivitiesLog> ActivitiesLogs { get; set; }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<BankBook> BankBooks { get; set; }

    public virtual DbSet<BankBookLog> BankBookLogs { get; set; }

    public virtual DbSet<BankSummary> BankSummaries { get; set; }

    public virtual DbSet<BankSummaryLog> BankSummaryLogs { get; set; }

    public virtual DbSet<BanksLog> BanksLogs { get; set; }

    public virtual DbSet<BeneficiariesLog> BeneficiariesLogs { get; set; }

    public virtual DbSet<Beneficiary> Beneficiaries { get; set; }

    public virtual DbSet<Donor> Donors { get; set; }

    public virtual DbSet<DonorsLog> DonorsLogs { get; set; }

    public virtual DbSet<ExpensesDetail> ExpensesDetails { get; set; }

    public virtual DbSet<ExpensesDetailsLog> ExpensesDetailsLogs { get; set; }

    public virtual DbSet<FolderBank> FolderBanks { get; set; }

    public virtual DbSet<FolderBankLog> FolderBankLogs { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<PettyCashSummary> PettyCashSummaries { get; set; }

    public virtual DbSet<PettyCashSummaryLog> PettyCashSummaryLogs { get; set; }

    public virtual DbSet<Privilege> Privileges { get; set; }

    public virtual DbSet<PrivilegesLog> PrivilegesLogs { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectsLog> ProjectsLogs { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReportsLog> ReportsLogs { get; set; }

    public virtual DbSet<RevenuesDetail> RevenuesDetails { get; set; }

    public virtual DbSet<RevenuesDetailsLog> RevenuesDetailsLogs { get; set; }

    public virtual DbSet<Summary> Summaries { get; set; }

    public virtual DbSet<SummaryLog> SummaryLogs { get; set; }

    public virtual DbSet<TransfersFromU> TransfersFromUs { get; set; }

    public virtual DbSet<TransfersFromUsLog> TransfersFromUsLogs { get; set; }

    public virtual DbSet<TransfersSummary> TransfersSummaries { get; set; }

    public virtual DbSet<TransfersSummaryLog> TransfersSummaryLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersLog> UsersLogs { get; set; }

    public virtual DbSet<Volunteer> Volunteers { get; set; }

    public virtual DbSet<VolunteersActivitiesLog> VolunteersActivitiesLogs { get; set; }

    public virtual DbSet<VolunteersActivity> VolunteersActivities { get; set; }

    public virtual DbSet<VolunteersLog> VolunteersLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivitiesLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Activities_Log_pkey");

            entity.ToTable("Activities_Log", "Projects");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Activities_pkey");

            entity.ToTable("Activities", "Projects");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Project).WithMany(p => p.Activities)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Activities_ProjectId_fkey");
        });

        modelBuilder.Entity<Bank>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Banks_pkey");

            entity.ToTable("Banks", "Accounting");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<BankBook>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Bank_Book_pkey");

            entity.ToTable("Bank_Book", "Accounting");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.Beneficiarie).HasMaxLength(100);
            entity.Property(e => e.DollarExchange).HasColumnType("money");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PayrollNumber).HasMaxLength(20);

            entity.HasOne(d => d.Bank).WithMany(p => p.BankBooks)
                .HasForeignKey(d => d.BankId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Bank_Book_BankId_fkey");
        });

        modelBuilder.Entity<BankBookLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Bank_Book_Log_pkey");

            entity.ToTable("Bank_Book_Log", "Accounting");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<BankSummary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Bank_Summary_pkey");

            entity.ToTable("Bank_Summary", "Accounting");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.DollarExchange).HasColumnType("money");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Bank).WithMany(p => p.BankSummaries)
                .HasForeignKey(d => d.BankId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Bank_Summary_BankId_fkey");
        });

        modelBuilder.Entity<BankSummaryLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Bank_Summary_Log_pkey");

            entity.ToTable("Bank_Summary_Log", "Accounting");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<BanksLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Banks_Log_pkey");

            entity.ToTable("Banks_Log", "Accounting");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<BeneficiariesLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Beneficiaries_Log_pkey");

            entity.ToTable("Beneficiaries_Log", "Projects");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Beneficiary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Beneficiaries_pkey");

            entity.ToTable("Beneficiaries", "Projects");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Project).WithMany(p => p.Beneficiaries)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Beneficiaries_ProjectId_fkey");
        });

        modelBuilder.Entity<Donor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Donors_pkey");

            entity.ToTable("Donors", "Donors");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<DonorsLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Donors_Log_pkey");

            entity.ToTable("Donors_Log", "Donors");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<ExpensesDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ExpensesDetails_pkey");

            entity.ToTable("ExpensesDetails", "Accounting");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.DollarExchange).HasColumnType("money");
            entity.Property(e => e.Folder).HasColumnType("money");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<ExpensesDetailsLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("ExpensesDetails_Log_pkey");

            entity.ToTable("ExpensesDetails_Log", "Accounting");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<FolderBank>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Folder_Bank_pkey");

            entity.ToTable("Folder_Bank", "Accounting");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.DollarExchange).HasColumnType("money");
            entity.Property(e => e.Folder).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TransactionId).HasMaxLength(20);

            entity.HasOne(d => d.Bank).WithMany(p => p.FolderBanks)
                .HasForeignKey(d => d.BankId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Folder_Bank_BankId_fkey");
        });

        modelBuilder.Entity<FolderBankLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Folder_Bank_Log_pkey");

            entity.ToTable("Folder_Bank_Log", "Accounting");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("password_reset_tokens_pkey");

            entity.ToTable("password_reset_tokens", "Users");

            entity.Property(e => e.ExpiryDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Expiry_date");
            entity.Property(e => e.Token).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user");
        });

        modelBuilder.Entity<PettyCashSummary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PettyCash_Summary_pkey");

            entity.ToTable("PettyCash_Summary", "Accounting");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.DollarExchange).HasColumnType("money");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<PettyCashSummaryLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PettyCash_Summary_Log_pkey");

            entity.ToTable("PettyCash_Summary_Log", "Accounting");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Privilege>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Privileges_pkey");

            entity.ToTable("Privileges", "Users");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithMany(p => p.Privileges)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Privileges_UserId_fkey");
        });

        modelBuilder.Entity<PrivilegesLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Privileges_Log_pkey");

            entity.ToTable("Privileges_Log", "Users");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Projects_pkey");

            entity.ToTable("Projects", "Projects");

            entity.Property(e => e.Budget).HasColumnType("money");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.State).HasMaxLength(20);
        });

        modelBuilder.Entity<ProjectsLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Projects_Log_pkey");

            entity.ToTable("Projects_Log", "Projects");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Reports_pkey");

            entity.ToTable("Reports", "Projects");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Project).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Reports_ProjectId_fkey");
        });

        modelBuilder.Entity<ReportsLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Reports_Log_pkey");

            entity.ToTable("Reports_Log", "Projects");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<RevenuesDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("RevenuesDetails_pkey");

            entity.ToTable("RevenuesDetails", "Accounting");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.DollarExchange).HasColumnType("money");
            entity.Property(e => e.Folder).HasColumnType("money");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<RevenuesDetailsLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("RevenuesDetails_Log_pkey");

            entity.ToTable("RevenuesDetails_Log", "Accounting");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Summary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Summary_pkey");

            entity.ToTable("Summary", "Accounting");

            entity.Property(e => e.DollarExchange).HasColumnType("money");
            entity.Property(e => e.Expenses).HasColumnType("money");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Outflows).HasColumnType("money");
            entity.Property(e => e.Revenues).HasColumnType("money");
        });

        modelBuilder.Entity<SummaryLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Summary_Log_pkey");

            entity.ToTable("Summary_Log", "Accounting");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<TransfersFromU>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TransfersFromUS_pkey");

            entity.ToTable("TransfersFromUS", "Accounting");

            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.DepositedQs).HasColumnType("money");
            entity.Property(e => e.DollarExchange).HasColumnType("money");
            entity.Property(e => e.Folder).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TransfersFromUsLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("TransfersFromUS_Log_pkey");

            entity.ToTable("TransfersFromUS_Log", "Accounting");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
            entity.Property(e => e.TransfersFromUsid).HasColumnName("TransfersFromUSId");
        });

        modelBuilder.Entity<TransfersSummary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TransfersSummary_pkey");

            entity.ToTable("TransfersSummary", "Accounting");

            entity.Property(e => e.BankBox).HasColumnType("money");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NetIncome).HasColumnType("money");
            entity.Property(e => e.RetainedEarning).HasColumnType("money");
            entity.Property(e => e.TotalExpenses).HasColumnType("money");
            entity.Property(e => e.TotalIncome).HasColumnType("money");
        });

        modelBuilder.Entity<TransfersSummaryLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("TransfersSummary_Log_pkey");

            entity.ToTable("TransfersSummary_Log", "Accounting");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.ToTable("Users", "Users");

            entity.HasIndex(e => e.UserName, "UQ_UserName").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.JobPosition).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UserName).HasMaxLength(30);
        });

        modelBuilder.Entity<UsersLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Users_Log_pkey");

            entity.ToTable("Users_Log", "Users");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<Volunteer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Volunteers_pkey");

            entity.ToTable("Volunteers", "Projects");

            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Role).HasMaxLength(100);

            entity.HasOne(d => d.Project).WithMany(p => p.Volunteers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Volunteers_ProjectId_fkey");
        });

        modelBuilder.Entity<VolunteersActivitiesLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Volunteers_Activities_Log_pkey");

            entity.ToTable("Volunteers_Activities_Log", "Projects");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        modelBuilder.Entity<VolunteersActivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Volunteers_Activities_pkey");

            entity.ToTable("Volunteers_Activities", "Projects");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Activity).WithMany(p => p.VolunteersActivities)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Volunteers_Activities_ActivityId_fkey");

            entity.HasOne(d => d.Volunteer).WithMany(p => p.VolunteersActivities)
                .HasForeignKey(d => d.VolunteerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Volunteers_Activities_VolunteerId_fkey");
        });

        modelBuilder.Entity<VolunteersLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Volunteers_Log_pkey");

            entity.ToTable("Volunteers_Log", "Projects");

            entity.Property(e => e.ChangedData).HasColumnType("jsonb");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.OperationType).HasMaxLength(10);
            entity.Property(e => e.PerformedBy).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
