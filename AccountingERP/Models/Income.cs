namespace AccountingERP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Income")]
    public partial class Income
    {
        public int IncomeID { get; set; }

        public double Amount { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public DateTime? EventDate { get; set; }

        public int? FirmUserID { get; set; }

        public int? ExpenseTypesID { get; set; }

        public int? FirmID { get; set; }

        public virtual ExpenseType ExpenseType { get; set; }

        public virtual Firm Firm { get; set; }

        public virtual FirmUser FirmUser { get; set; }
    }
}
