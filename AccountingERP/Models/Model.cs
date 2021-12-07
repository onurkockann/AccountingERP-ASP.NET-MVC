using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace AccountingERP.Models
{
    public partial class Model : DbContext
    {
        public Model()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Debt> Debts { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<ExpenseType> ExpenseTypes { get; set; }
        public virtual DbSet<Firm> Firms { get; set; }
        public virtual DbSet<FirmUser> FirmUsers { get; set; }
        public virtual DbSet<Income> Incomes { get; set; }
        public virtual DbSet<MTIPartner> MTIPartners { get; set; }
        public virtual DbSet<Payee> Payees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Debt>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Expense>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Firm>()
                .HasMany(e => e.Articles)
                .WithOptional(e => e.Firm)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Firm>()
                .HasMany(e => e.Debts)
                .WithOptional(e => e.Firm)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Firm>()
                .HasMany(e => e.Expenses)
                .WithOptional(e => e.Firm)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Firm>()
                .HasMany(e => e.FirmUsers)
                .WithOptional(e => e.Firm)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Firm>()
                .HasMany(e => e.Incomes)
                .WithOptional(e => e.Firm)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Firm>()
                .HasMany(e => e.MTIPartners)
                .WithOptional(e => e.Firm)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Firm>()
                .HasMany(e => e.Payees)
                .WithOptional(e => e.Firm)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FirmUser>()
                .HasMany(e => e.Expenses)
                .WithOptional(e => e.FirmUser1)
                .HasForeignKey(e => e.FirmUser);

            modelBuilder.Entity<FirmUser>()
                .HasMany(e => e.MTIPartners)
                .WithOptional(e => e.FirmUser)
                .HasForeignKey(e => e.ReceiverID);

            modelBuilder.Entity<FirmUser>()
                .HasMany(e => e.MTIPartners1)
                .WithOptional(e => e.FirmUser1)
                .HasForeignKey(e => e.SenderID);

            modelBuilder.Entity<Income>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Payee>()
                .Property(e => e.Description)
                .IsUnicode(false);
        }
    }
}
