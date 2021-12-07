namespace AccountingERP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Payee
    {
        [Key]
        public int PayeesID { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        public double Amount { get; set; }

        public int? FirmID { get; set; }

        public virtual Firm Firm { get; set; }
    }
}
