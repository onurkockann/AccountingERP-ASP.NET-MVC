namespace AccountingERP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MTIPartner
    {
        [Key]
        public int MTIPartnersID { get; set; }

        public int? SenderID { get; set; }

        public int? ReceiverID { get; set; }

        public double Amount { get; set; }

        public DateTime? EventDate { get; set; }

        public int? EventState { get; set; }

        public int? FirmID { get; set; }

        public virtual Firm Firm { get; set; }

        public virtual FirmUser FirmUser { get; set; }

        public virtual FirmUser FirmUser1 { get; set; }
    }
}
