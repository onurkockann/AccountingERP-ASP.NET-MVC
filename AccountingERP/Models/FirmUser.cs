namespace AccountingERP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FirmUser")]
    public partial class FirmUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FirmUser()
        {
            Articles = new HashSet<Article>();
            Expenses = new HashSet<Expense>();
            Incomes = new HashSet<Income>();
            MTIPartners = new HashSet<MTIPartner>();
            MTIPartners1 = new HashSet<MTIPartner>();
        }

        public int FirmUserID { get; set; }

        [Required]
        [StringLength(25)]
        public string username { get; set; }

        [Required]
        [StringLength(256)]
        public string pw { get; set; }

        [Required]
        [StringLength(35)]
        public string mail { get; set; }

        [StringLength(25)]
        public string Phone { get; set; }

        [StringLength(25)]
        public string name { get; set; }

        [StringLength(30)]
        public string surname { get; set; }

        public DateTime? DateJoin { get; set; }

        public double? NetState { get; set; }

        public int Rank { get; set; }

        public int? FirmID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Article> Articles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Expense> Expenses { get; set; }

        public virtual Firm Firm { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Income> Incomes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTIPartner> MTIPartners { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTIPartner> MTIPartners1 { get; set; }
    }
}
