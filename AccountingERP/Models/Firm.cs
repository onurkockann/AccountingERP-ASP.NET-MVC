namespace AccountingERP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Firm")]
    public partial class Firm
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Firm()
        {
            Articles = new HashSet<Article>();
            Debts = new HashSet<Debt>();
            Expenses = new HashSet<Expense>();
            FirmUsers = new HashSet<FirmUser>();
            Incomes = new HashSet<Income>();
            MTIPartners = new HashSet<MTIPartner>();
            Payees = new HashSet<Payee>();
        }

        public int FirmID { get; set; }

        [Required]
        [StringLength(25)]
        public string FirmName { get; set; }

        [StringLength(100)]
        public string Adres { get; set; }

        [StringLength(25)]
        public string Phone { get; set; }

        [StringLength(25)]
        public string WebSite { get; set; }

        public int? PartnerCount { get; set; }

        public double? NetProfit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Article> Articles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Debt> Debts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Expense> Expenses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FirmUser> FirmUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Income> Incomes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTIPartner> MTIPartners { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payee> Payees { get; set; }
    }
}
